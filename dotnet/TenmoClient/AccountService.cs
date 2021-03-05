using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient
{
    class AccountService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly static string ACCOUNTS_URL = API_BASE_URL + "accounts";
        private readonly IRestClient client = new RestClient();
        public decimal GetAccountBalance()
        {
            string token = UserService.GetToken();
            client.Authenticator = new JwtAuthenticator(token);

            RestRequest request = new RestRequest(ACCOUNTS_URL);
            IRestResponse<decimal> response = client.Get<decimal>(request);

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
