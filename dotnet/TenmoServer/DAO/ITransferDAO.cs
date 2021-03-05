using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        public bool SubtractFromBalance(int fromUserId, decimal transferAmount);
        public bool AddToBalance(int toUserId, decimal transferAmount);
        public Transfer CreateTransfer(Transfer transfer);
    }
}
