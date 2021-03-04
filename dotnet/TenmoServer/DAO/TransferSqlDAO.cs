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

        public void CreateTransfer(int fromUserId, int toUserId, decimal transferAmount)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string insertSql = "INSERT INTO transfers(transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES (2, 2, (SELECT account_id FROM accounts WHERE user_id = @fromUserId), (SELECT account_id FROM accounts WHERE user_id = @toUserId), @transferAmount);";
                    SqlCommand cmd = new SqlCommand(insertSql, conn);
                    cmd.Parameters.AddWithValue("@fromUserId", fromUserId);
                    cmd.Parameters.AddWithValue("@toUserId", toUserId);
                    cmd.Parameters.AddWithValue("@transferAmount", transferAmount);
                    cmd.ExecuteNonQuery();

                    string selectSql = "SELECT account_from, account_to, amount FROM transfers WHERE transfer_id = @@IDENTITY;";
                    cmd = new SqlCommand(selectSql, conn);

                }
            }
            catch (SqlException)
            {
                throw;
            }

            
        }
    }
}
