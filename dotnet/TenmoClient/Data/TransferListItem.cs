using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class TransferListItem
    {
        public int TransferId { get; set; }
        public string Username { get; set; }
        public decimal TransferAmount { get; set; }
        public int TransferType { get; set; }
    }
}
