using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly IUserDAO _userDAO;
        private readonly ITransferDAO _transferDAO;

        public TransferController(IUserDAO userDAO = null, ITransferDAO transferDAO = null)
        {
            _userDAO = userDAO;
            _transferDAO = transferDAO;
        }
        
        [HttpGet]
        public ActionResult<List<User>> GetAllUsersForTransfer()
        {
            List<User> users = _userDAO.GetUsers();

            if(users != null)
            {
                return Ok(users);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public ActionResult<List<TransferListItem>> GetTransfers(int userId)
        {
            List<TransferListItem> alltransfers = _transferDAO.GetTransfers(userId);

            if (alltransfers != null)
            {
                return Ok(alltransfers);
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost]
        public ActionResult<Transfer> CreateTransfer(Transfer transfer)
        {            
            Transfer newTransfer = _transferDAO.CreateTransfer(transfer);
            bool isSubtracted = _transferDAO.SubtractFromBalance(transfer.AccountFrom, transfer.TransferAmount);
            bool isAdded = _transferDAO.AddToBalance(transfer.AccountTo, transfer.TransferAmount);

            if (isSubtracted && isAdded && newTransfer != null)
            {
                return Ok(newTransfer);
            }
            else
            {
                return BadRequest("nope, sorry");
            }
        }
    }
}
