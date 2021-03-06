using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly TransferService transferService = new TransferService();
        private static readonly AccountService accountService = new AccountService();

        static void Main(string[] args)
        {
            Run();
        }

        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        API_User user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                        }
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }

            MenuSelection();
        }

        private static void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    decimal balance = accountService.GetAccountBalance();
                    Console.WriteLine();
                    Console.WriteLine($"Your current balance is: ${balance}");
                }
                else if (menuSelection == 2)
                {
                    int userId = UserService.GetUserId();
                    List<TransferListItem> transferList = transferService.GetListOfAllTransfers(userId);

                    string transferType = "";

                    foreach (TransferListItem transferListItem in transferList)
                    {
                        if (transferListItem.TransferType == 1)
                        {
                            transferType = "From";
                        }
                        else
                        {
                            transferType = "To";
                        }

                        Console.WriteLine($"Transfer ID: {transferListItem.TransferId} {transferType}: {transferListItem.Username} Amount: $ {transferListItem.TransferAmount}");
                    }
                }
                else if (menuSelection == 3)
                {

                }
                else if (menuSelection == 4)
                {
                    int fromUserId = UserService.GetUserId();
                    decimal balance = accountService.GetAccountBalance();
                    List<User> users = transferService.GetAllUsersForTransfer();

                    foreach (User user in users)
                    {
                        Console.WriteLine($"Username is: {user.Username} UserId is: {user.UserId}");
                    }

                    //get the user id the money is being sent to
                    Console.WriteLine();
                    Console.Write("Enter ID of user you are sending to (0 to cancel): ");
                    int toUserId = Convert.ToInt32(Console.ReadLine());

                    //get amount being sent
                    Console.Write("Enter amount: ");
                    decimal transferAmount = Convert.ToDecimal(Console.ReadLine());

                    if (balance >= transferAmount)
                    {
                        Transfer newTransfer = transferService.MakeTransferFromUserInput(fromUserId, toUserId, transferAmount);
                        transferService.CreateTransfer(newTransfer);
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Insufficient funds. Your current balance is ${balance}");
                    }                    
                }
                else if (menuSelection == 5)
                {

                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                    Run(); //return to entry point
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
