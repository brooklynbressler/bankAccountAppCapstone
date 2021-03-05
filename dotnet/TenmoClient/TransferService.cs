using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient
{
    public class TransferService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly static string TRANSFER_URL = API_BASE_URL + "transfer";
        private readonly IRestClient client = new RestClient();

        public List<TransferListItem> GetListOfAllTransfers(int userId)
        {
            string token = UserService.GetToken();
            client.Authenticator = new JwtAuthenticator(token);

            RestRequest request = new RestRequest(TRANSFER_URL + "/{id}");
            IRestResponse<List<TransferListItem>> response = client.Get<List<TransferListItem>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Error - Unable to reach server", response.ErrorException);
            }
            else if (!response.IsSuccessful)
            {
                throw new Exception("Error - Received unsuccessful response", response.ErrorException);
            }
            else
            {
                return response.Data;
            }
        }

        public Transfer MakeTransferFromUserInput(int accountFromId, int accountToId, decimal transferAmount)
        {
            string token = UserService.GetToken();
            client.Authenticator = new JwtAuthenticator(token);

            Transfer returnTransfer = new Transfer();

            returnTransfer.AccountFrom = accountFromId;
            returnTransfer.AccountTo = accountToId;
            returnTransfer.TransferAmount = transferAmount;

            return returnTransfer;
        }

        public Transfer CreateTransfer(Transfer transfer)
        {
            string token = UserService.GetToken();
            client.Authenticator = new JwtAuthenticator(token);

            RestRequest request = new RestRequest(TRANSFER_URL);
            request.AddJsonBody(transfer);
            IRestResponse<Transfer> response = client.Post<Transfer>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Error - Unable to reach server", response.ErrorException);
            }
            else if (!response.IsSuccessful)
            {
                throw new Exception("Error - Received unsuccessful response", response.ErrorException);
            }
            else
            {
                return response.Data;
            }
        }

        public List<User> GetAllUsersForTransfer()
        {
            string token = UserService.GetToken();
            client.Authenticator = new JwtAuthenticator(token);

            RestRequest request = new RestRequest(TRANSFER_URL);
            IRestResponse<List<User>> response = client.Get<List<User>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Error - Unable to reach server", response.ErrorException);
            }
            else if (!response.IsSuccessful)
            {
                throw new Exception("Error - Received unsuccessful response", response.ErrorException);
            }
            else
            {
                return response.Data;
            }
        }
    }
}
