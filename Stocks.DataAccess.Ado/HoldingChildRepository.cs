using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stocks.Domain;
using System.Data.SqlClient;

namespace Stocks.DataAccess.Ado
{
    public class HoldingChildRepository
    {

        #region PersistChild
        public Holding PersistChild(Holding holding, SqlConnection conn)
        {
            if (holding.HoldingId == 0 && holding.IsMarkedForDeletion)
            {
                holding = null;
            }
            else if (holding.IsMarkedForDeletion)
            {
                DeleteEntity(holding, conn);
                holding = null;
            }
            else if (holding.HoldingId == 0)
            {
                InsertEntity(holding, conn);
                holding.IsDirty = false;
            }
            else if (holding.IsDirty)
            {
                UpdateEntity(holding, conn);
                holding.IsDirty = false;
            }
            return holding;

        }
        #endregion

        #region SQL

        internal static void InsertEntity(Holding item, SqlConnection conn)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                var sql = new StringBuilder();
                sql.Append("insert Holding (ClientId, StockId, Quantity, LastChangeDate)");
                sql.Append("values (@ClientId, @StockId, @Quantity, @LastChangeDate);");
                sql.Append("select cast ( scope_identity() as int);");
                cmd.CommandText = sql.ToString();

                SetCommonParameters(item, cmd);
                item.HoldingId = (int)cmd.ExecuteScalar();
            }
        }

        internal static void UpdateEntity(Holding item, SqlConnection conn)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                var sql = new StringBuilder();
                sql.Append("update Holding set ");
                sql.Append(" ClientId = @ClientId, ");
                sql.Append(" StockId = @StockId, ");
                sql.Append(" Quantity = @Quantity, ");
                sql.Append(" LastChangeDate = @LastChangeDate ");

                sql.Append("where HoldingId = @HoldingId");
                cmd.CommandText = sql.ToString();

                SetCommonParameters(item, cmd);
                cmd.Parameters.AddWithValue("@HoldingId", item.HoldingId);

                cmd.ExecuteNonQuery();
            }
        }

        internal static void DeleteEntity(Holding item, SqlConnection conn)
        {
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "delete Holding where HoldingId = @HoldingId";
                cmd.Parameters.AddWithValue("@HoldingId", item.HoldingId);
                var i = cmd.ExecuteNonQuery();
            }
        }

        private static void SetCommonParameters(Holding item, SqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@ClientId", item.ClientId);
            cmd.Parameters.AddWithValue("@StockId", item.StockId);
            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
            cmd.Parameters.AddWithValue("@LastChangeDate", item.LastChangeDate);
        }

        #endregion
    }
}
