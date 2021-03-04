using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient
{
    public class TransferService
    {
        public Transfer MakeTransferFromUserInput(int accountFromId, int accountToId, decimal transferAmount)
        {
            Transfer returnTransfer = new Transfer();

            returnTransfer.AccountFrom = accountFromId;
            returnTransfer.AccountTo = accountToId;
            returnTransfer.TransferAmount = transferAmount;

            return returnTransfer;
        }

    }
}
