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

        public TransferReceipt GetTransferReceipt(int transferId)
        {
            TransferReceipt transferReceipt = new TransferReceipt();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "SELECT t.transfer_id AS TransferId, t.account_from AS AccountFrom, t.account_to AS AccountTo, t.transfer_type_id AS TransferType, ts.transfer_status_desc AS Status, t.amount AS Amount " +
                        "FROM transfers t " +
                        "JOIN accounts a ON a.account_id = t.account_from " +
                        "JOIN transfer_types tt ON tt.transfer_type_id = t.transfer_type_id " +
                        "JOIN users u ON u.user_id = a.user_id " +
                        "JOIN transfer_statuses ts ON ts.transfer_status_id = t.transfer_status_id " +
                        "WHERE t.transfer_id = @transferId;";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@transferId", transferId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        transferReceipt = GetTransferReceiptFromReader(reader);
                    }
                }
            }
            catch(SqlException)
            {
                throw;
            }

            return transferReceipt;
        }

        public List<TransferListItem> GetTransfers(int userId)
        {
            List<TransferListItem> transferList = new List<TransferListItem>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = "SELECT t.transfer_id AS TransferId, u.username AS Username, t.amount AS TransferAmount, tt.transfer_type_id AS TransferType " +
                        "FROM accounts a " +
                        "JOIN transfers t ON a.account_id = t.account_to " +
                        "JOIN users u ON a.user_id = u.user_id " +
                        "JOIN transfer_types tt ON t.transfer_type_id = tt.transfer_type_id;";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        TransferListItem tli = GetTransferListItemFromReader(reader);
                        transferList.Add(tli);
                    }

                }
            }
            catch (SqlException)
            {
                throw;
            }

            return transferList;
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

                    string sql = "SELECT transfer_id, account_from, account_to, amount FROM transfers WHERE transfer_id = @transferId; ";
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
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                AccountFrom = Convert.ToInt32(reader["account_from"]),
                AccountTo = Convert.ToInt32(reader["account_to"]),
                TransferAmount = Convert.ToDecimal(reader["amount"])
            };

            return t;
        }
    
        private TransferListItem GetTransferListItemFromReader(SqlDataReader reader)
        {
            TransferListItem tli = new TransferListItem()
            {
                TransferId = Convert.ToInt32(reader["TransferId"]),
                Username = Convert.ToString(reader["Username"]),
                TransferAmount = Convert.ToDecimal(reader["TransferAmount"]),
                TransferType = Convert.ToInt32(reader["TransferType"])
            };

            return tli;
        }

        private TransferReceipt GetTransferReceiptFromReader(SqlDataReader reader)
        {
            TransferReceipt tr = new TransferReceipt()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                FromUserId = Convert.ToInt32(reader["account_from"]),
                ToUserId = Convert.ToInt32(reader["account_to"]),
                TransferType = Convert.ToInt32(reader["transfer_type_id"]),
                TransferStatus = Convert.ToString(reader["transfer_status_id"]),
                Amount = Convert.ToDecimal(reader["amount"])
            };

            return tr;
        }
    }
}
