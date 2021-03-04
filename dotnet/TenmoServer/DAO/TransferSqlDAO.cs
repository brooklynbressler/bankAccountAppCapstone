using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDAO : ITransferDAO
    {
        private readonly string connectionString;

        public TransferSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public bool SubtractFromBalance(int fromUserId, decimal transferAmount)
        {
            bool isSuccessful = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "UPDATE accounts SET balance = (balance - @transferAmount) WHERE user_id = @fromUserId;";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@fromUserId", fromUserId);                    
                    cmd.Parameters.AddWithValue("@transferAmount", transferAmount);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        isSuccessful = true;
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return isSuccessful;
        }

        public bool AddToBalance(int toUserId, decimal transferAmount)
        {
            bool isSuccessful = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "UPDATE accounts SET balance = (balance + @transferAmount) WHERE user_id = @toUserId;";
                    SqlCommand cmd = new SqlCommand(sql, conn);                    
                    cmd.Parameters.AddWithValue("@toUserId", toUserId);
                    cmd.Parameters.AddWithValue("@transferAmount", transferAmount);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        isSuccessful = true;
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return isSuccessful;
        }

        public Transfer GetTransfer(int tranferId)
        {
            Transfer transfer = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "SELECT account_from, account_to, amount FROM transfers WHERE transfer_id = @transferId; ";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@transferId", tranferId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        transfer = GetTransferFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {

                throw;
            }

            return transfer;
        }

        public Transfer CreateTransfer(Transfer transfer)
        {
            Transfer returnTransfer = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string insertSql = "INSERT INTO transfers(transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES (2, 2, (SELECT account_id FROM accounts WHERE user_id = @fromUserId), (SELECT account_id FROM accounts WHERE user_id = @toUserId), @transferAmount); SELECT SCOPE_IDENTITY();";
                    SqlCommand cmd = new SqlCommand(insertSql, conn);
                    cmd.Parameters.AddWithValue("@fromUserId", transfer.AccountFrom);
                    cmd.Parameters.AddWithValue("@toUserId", transfer.AccountTo);
                    cmd.Parameters.AddWithValue("@transferAmount", transfer.TransferAmount);

                    int rowId = Convert.ToInt32(cmd.ExecuteScalar());
                    transfer.TransferId = rowId;

                    returnTransfer = GetTransfer(transfer.TransferId);
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnTransfer;
        }

        private Transfer GetTransferFromReader(SqlDataReader reader)
        {
            Transfer t = new Transfer()
            {
                AccountFrom = Convert.ToInt32(reader["account_from"]),
                AccountTo = Convert.ToInt32(reader["account_to"]),
                TransferAmount = Convert.ToDecimal(reader["amount"])
            };

            return t;
        }
    }
}
