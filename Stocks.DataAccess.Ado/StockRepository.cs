using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ucla.Common.ExtensionMethods;
using Ucla.Common.Interfaces;
using Ucla.Common.Utility;
using Stocks.Domain;
using System.Configuration;
using System.Data.SqlClient;
using System.Transactions;

namespace Stocks.DataAccess.Ado
{
    public class StockRepository : IRepository<Stock>
    {
        #region IRepository<Stock> members

        /// <summary> 
        /// Saves entity changes to the database 
        /// </summary> 
        /// <param name="item"></param> 
        /// <returns>updated entity, or null if the entity is deleted</returns>
        public IEnumerable<Stock> Fetch(object criteria = null)
        {
            var data = new List<Stock>();
            var connString = ConfigurationManager
                .ConnectionStrings["AppConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    if (criteria == null)
                    {
                        var sql = new StringBuilder();
                        sql.Append("select * from Stock; ");
                        sql.Append("select * from Holding; ");
                        cmd.CommandText = sql.ToString();
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            var s = new Stock();
                            s.StockId = dr.AsInt32("StockId");
                            s.Code = dr.AsString("Code");
                            s.CompanyName = dr.AsString("CompanyName");
                            s.LastPrice = dr.AsDecimal("LastPrice");

                            data.Add(s);
                        }

                        dr.NextResult();
                        while (dr.Read())
                        {
                            var h = new Holding();
                            h.HoldingId = dr.AsInt32("HoldingId");
                            h.ClientId = dr.AsInt32("ClientId");
                            h.StockId = dr.AsInt32("StockId");
                            h.Quantity = dr.AsInt64("Quantity");
                            h.LastChangeDate = dr.AsDateTime("LastChangeDate");

                            data.Where(o => o.StockId == h.StockId)
                                .Single().Holdings.Add(h);
                        }
                    }
                    else if (criteria is int)
                    {
                        var sql = new StringBuilder();
                        sql.Append("select * from Stock where StockId = @StockId; \r\n");
                        sql.Append("select * from Holding where StockId = @StockId; \r\n");
                        cmd.CommandText = sql.ToString();
                        cmd.Parameters.AddWithValue("@StockId", (int)criteria);
                        var dr = cmd.ExecuteReader();
                        var s = new Stock();
                        while (dr.Read())
                        {
                            s.StockId = dr.AsInt32("StockId");
                            s.Code = dr.AsString("Code");
                            s.CompanyName = dr.AsString("CompanyName");
                            s.LastPrice = dr.AsDecimal("LastPrice");

                            data.Add(s);
                        }

                        dr.NextResult();
                        while (dr.Read())
                        {
                            var h = new Holding();
                            h.HoldingId = dr.AsInt32("HoldingId");
                            h.ClientId = dr.AsInt32("ClientId");
                            h.StockId = dr.AsInt32("StockId");
                            h.Quantity = dr.AsInt64("Quantity");
                            h.LastChangeDate = dr.AsDateTime("LastChangeDate");
                            s.Holdings.Add(h);
                        }
                    }
                    else
                    {
                        var msg = String.Format("ShowRepository: Unknown criteria type: {0}", criteria);
                        throw new InvalidOperationException(msg);
                    }
                }
            } return data;
        }

        /// <summary> 
        /// Saves entity changes to the database 
        /// </summary> 
        /// <param name="item"></param> 
        /// <returns>updated entity, or null if the entity is deleted</returns>
        public Stock Persist(Stock item)
        {
            if (item.StockId == 0 && item.IsMarkedForDeletion)
            {
                item = null;
            }

            var connString = ConfigurationManager
                .ConnectionStrings["AppConnection"].ConnectionString;
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        conn.Open();
                        if (item.IsMarkedForDeletion)
                        { // Also Deletes Children 
                            DeleteEntity(item, conn);
                            item = null;
                        }
                        else if (item.StockId == 0)
                        {
                            InsertEntity(item, conn);
                            PersistChildren(item, conn);
                            item.IsDirty = false;
                        }
                        else if (item.IsDirty)
                        {
                            UpdateEntity(item, conn);
                            PersistChildren(item, conn);
                            item.IsDirty = false;
                        }
                        else
                        { // No changes to show, but might be changes to children 
                            PersistChildren(item, conn);
                        }
                    }
                    ts.Complete();
                }
            }
            catch (SqlException ex)
            {
                var msg = SqlExceptionDecoder.GetFriendlyMessage("Show", ex);
                throw new ApplicationException(msg, ex);
            }
            return item;
        }

        private static void PersistChildren(Stock stock, SqlConnection conn)
        {
            if (stock.Holdings.Any())
            {
                var repo = new HoldingChildRepository();
                for (var index = stock.Holdings.Count() - 1; index >= 0; index--)
                {
                    stock.Holdings[index].StockId = stock.StockId;
                    var holding = repo.PersistChild(stock.Holdings[index], conn);
                    if (holding == null)
                    {
                        // Persist returns null, remove Holding from client
                        stock.Holdings.RemoveAt(index);
                    }
                    else
                    {
                        // For insert, replaces with object that has id assigned.
                        stock.Holdings[index] = holding;
                    }
                }
            }
        }

        #endregion

        #region SQL methods

        internal static void DeleteEntity(Stock item, SqlConnection conn)
        {
            // Cascade delete Holdings
            foreach (var holding in item.Holdings)
            {
                HoldingChildRepository.DeleteEntity(holding, conn);
            }

            // Delete Stock itself
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "delete Stock where StockId = @StockId";
                cmd.Parameters.AddWithValue("@StockId", item.StockId);
                cmd.ExecuteNonQuery();
            }
        }

        internal static void InsertEntity(Stock item, SqlConnection conn)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                var sql = new StringBuilder();
                sql.Append("insert Stock (Code, CompanyName, "
                    + "LastPrice)");
                sql.Append("values ( @Code, @CompanyName, "
                    + "@LastPrice);");
                sql.Append("select cast ( scope_identity() as int);");
                cmd.CommandText = sql.ToString();

                SetCommonParameters(item, cmd);
                item.StockId = (int)cmd.ExecuteScalar();
            }
        }

        internal static void UpdateEntity(Stock item, SqlConnection conn)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                var sql = new StringBuilder();
                sql.Append("update Stock set ");
                sql.Append(" Code = @Code, ");
                sql.Append(" CompanyName = @CompanyName, ");
                sql.Append(" LastPrice = @LastPrice ");
                sql.Append("where StockId = @StockId");
                cmd.CommandText = sql.ToString();

                SetCommonParameters(item, cmd);
                cmd.Parameters.AddWithValue("@StockId", item.StockId);

                cmd.ExecuteNonQuery();
            }
        }

        private static void SetCommonParameters(Stock item, SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@Code", item.Code);
            cmd.Parameters.AddWithValue("@CompanyName", item.CompanyName);
            cmd.Parameters.AddWithValue("@LastPrice", item.LastPrice);
        }


        #endregion

    }
}
