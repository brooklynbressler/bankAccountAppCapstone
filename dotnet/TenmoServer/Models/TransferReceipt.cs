using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class TransferReceipt
    {
        public int TransferId { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public int TransferType { get; set; }
        public string TransferStatus { get; set; }
        public decimal Amount { get; set; }

    }
}
