using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    public class AuthService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly static string ACCOUNTS_URL = API_BASE_URL + "accounts";
        private readonly static string TRANSFER_URL = API_BASE_URL + "transfer";
        private readonly IRestClient client = new RestClient();

        public Transfer CreateTransfer(Transfer transfer)
        {
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

        public User GetUser(int userId)
        {
            RestRequest request = new RestRequest($"{TRANSFER_URL}/{userId}");
            IRestResponse<User> response = client.Get<User>(request);

            if(response.ResponseStatus != ResponseStatus.Completed)
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

        public List<User> GetAllUsers()
        {
            RestRequest request = new RestRequest(TRANSFER_URL);
            IRestResponse<List<User>> response = client.Get<List<User>>(request);

            if(response.ResponseStatus != ResponseStatus.Completed)
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

        public decimal GetAccountBalance()
        {
            RestRequest request = new RestRequest(ACCOUNTS_URL);
            IRestResponse<decimal> response = client.Get<decimal>(request);

            if(response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Error - Unable to reach server", response.ErrorException);
            }
            else if(!response.IsSuccessful)
            {
                throw new Exception("Error - Received unsuccessful response", response.ErrorException);
            }
            else
            {
                return response.Data;
            }
        }

        //login endpoints
        public bool Register(LoginUser registerUser)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "login/register");
            request.AddJsonBody(registerUser);
            IRestResponse<API_User> response = client.Post<API_User>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return false;
            }
            else if (!response.IsSuccessful)
            {
                if (!string.IsNullOrWhiteSpace(response.Data.Message))
                {
                    Console.WriteLine("An error message was received: " + response.Data.Message);
                }
                else
                {
                    Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public API_User Login(LoginUser loginUser)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "login");
            request.AddJsonBody(loginUser);
            IRestResponse<API_User> response = client.Post<API_User>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                if (!string.IsNullOrWhiteSpace(response.Data.Message))
                {
                    Console.WriteLine("An error message was received: " + response.Data.Message);
                }
                else
                {
                    Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                }
                return null;
            }
            else
            {
                client.Authenticator = new JwtAuthenticator(response.Data.Token);
                return response.Data;
            }
        }

    }
}
