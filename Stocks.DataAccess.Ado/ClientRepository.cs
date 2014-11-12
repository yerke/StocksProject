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
    public class ClientRepository : IRepository<Client>
    {
        #region IRepository<Client> members

        /// <summary> 
        /// Saves entity changes to the database 
        /// </summary> 
        /// <param name="item"></param> 
        /// <returns>updated entity, or null if the entity is deleted</returns>
        public IEnumerable<Client> Fetch(object criteria = null)
        {
            var data = new List<Client>();
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
                        sql.Append("select * from Client; "); 
                        sql.Append("select * from Holding; "); 
                        cmd.CommandText = sql.ToString(); 
                        var dr = cmd.ExecuteReader(); 
                        while (dr.Read()) 
                        { 
                            var c = new Client(); 
                            c.ClientId = dr.AsInt32("ClientId");
                            c.Code = dr.AsString("Code");
                            c.FirstName = dr.AsString("FirstName");
                            c.LastName = dr.AsString("LastName");
                            c.Phone = dr.AsString("Phone");
                            c.Address = dr.AsString("Address");
                            c.IsDirty = false;

                            data.Add(c); 
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
                            h.IsDirty = false;

                            data.Where(o => o.ClientId == h.ClientId)
                                .Single().Holdings.Add(h); 
                        }
                    }
                    else if (criteria is ClientCriteria)
                    {
                        var sql = new StringBuilder();
                        sql.Append(@"select * from Client 
                        where FirstName like '%' + @Name + '%' 
                        or LastName like '%' + @Name + '%';");
                        sql.Append(@"select * from Holding h 
                        join Client c on h.ClientId = c.ClientId 
                        where c.FirstName like '%' + @Name + '%' 
                        or c.LastName like '%' + @Name + '%';");
                        cmd.CommandText = sql.ToString();
                        cmd.Parameters.AddWithValue("@Name", ((ClientCriteria)criteria).Name);
                        var dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            var c = new Client();
                            c.ClientId = dr.AsInt32("ClientId");
                            c.Code = dr.AsString("Code");
                            c.FirstName = dr.AsString("FirstName");
                            c.LastName = dr.AsString("LastName");
                            c.Phone = dr.AsString("Phone");
                            c.Address = dr.AsString("Address");
                            c.IsDirty = false;

                            data.Add(c);
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
                            h.IsDirty = false;

                            data.Where(o => o.ClientId == h.ClientId)
                                .Single().Holdings.Add(h);
                        }

                    }
                    else if (criteria is int)
                    {
                        var sql = new StringBuilder(); 
                        sql.Append("select * from Client where ClientId = @ClientId; \r\n");
                        sql.Append("select * from Holding where ClientId = @ClientId; \r\n"); 
                        cmd.CommandText = sql.ToString(); 
                        cmd.Parameters.AddWithValue("@ClientId", (int)criteria); 
                        var dr = cmd.ExecuteReader();
                        var c = new Client(); 
                        while (dr.Read()) 
                        {
                            c.ClientId = dr.AsInt32("ClientId");
                            c.Code = dr.AsString("Code");
                            c.FirstName = dr.AsString("FirstName");
                            c.LastName = dr.AsString("LastName");
                            c.Phone = dr.AsString("Phone");
                            c.Address = dr.AsString("Address");
                            c.IsDirty = false;

                            data.Add(c);
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
                            h.IsDirty = false;
                            c.Holdings.Add(h); 
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
        public Client Persist(Client item)
        {
            if (item.ClientId == 0 && item.IsMarkedForDeletion)
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
                        else if (item.ClientId == 0)
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

        private static void PersistChildren(Client client, SqlConnection conn)
        {

            if (client.Holdings.Any())
            {
                var repo = new HoldingChildRepository();
                for (var index = client.Holdings.Count() - 1; index >= 0; index--)
                {
                    client.Holdings[index].ClientId = client.ClientId;
                    var holding = repo.PersistChild(client.Holdings[index], conn);
                    if (holding == null)
                    {
                        // Persist returns null, remove Holding from client
                        client.Holdings.RemoveAt(index);
                    }
                    else
                    {
                        // For insert, replaces with object that has id assigned.
                        client.Holdings[index] = holding;
                    }
                }
            }
        }

        #endregion

        #region SQL methods

        internal static void DeleteEntity(Client item, SqlConnection conn)
        {
            // Cascade delete Holdings
            foreach (var holding in item.Holdings)
            {
                HoldingChildRepository.DeleteEntity(holding, conn);
            }

            // Delete Client itself
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "delete Client where ClientId = @ClientId";
                cmd.Parameters.AddWithValue("@ClientId", item.ClientId);
                var i = cmd.ExecuteNonQuery();
            }
        }

        internal static void InsertEntity(Client item, SqlConnection conn)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                var sql = new StringBuilder();
                sql.Append("insert Client (Code, FirstName, "
                    + "LastName, Phone, Address)");
                sql.Append("values ( @Code, @FirstName, "
                    + "@LastName, @Phone, @Address);");
                sql.Append("select cast ( scope_identity() as int);");
                cmd.CommandText = sql.ToString();

                SetCommonParameters(item, cmd);
                item.ClientId = (int)cmd.ExecuteScalar();
            }
        }

        internal static void UpdateEntity(Client item, SqlConnection conn)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                var sql = new StringBuilder();
                sql.Append("update Client set ");
                sql.Append(" Code = @Code, ");
                sql.Append(" FirstName = @FirstName, ");
                sql.Append(" LastName = @LastName, ");
                sql.Append(" Phone = @Phone, ");
                sql.Append(" Address = @Address ");
                sql.Append("where ClientId = @ClientId");
                cmd.CommandText = sql.ToString();

                SetCommonParameters(item, cmd);
                cmd.Parameters.AddWithValue("@ClientId", item.ClientId);

                cmd.ExecuteNonQuery();
            }
        }

        private static void SetCommonParameters(Client item, SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@Code", item.Code);
            cmd.Parameters.AddWithValue("@FirstName", item.FirstName);
            cmd.Parameters.AddWithValue("@LastName", item.LastName);
            cmd.Parameters.AddWithValue("@Phone", item.Phone);
            cmd.Parameters.AddWithValue("@Address", item.Address);
        }


        #endregion

    }
}
