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
        public ActionResult<List<User>> GetAllUsers()
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
        
        [HttpPut]
        public ActionResult<bool> UpdateAccountBalance(int fromUserId, int toUserId, decimal transferAmount)
        {
            bool isUpdated = _transferDAO.UpdateBalances(fromUserId, toUserId, transferAmount);

            if (isUpdated)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("{fromUserId}/{toUserId}/{transferAmount}")]
        public ActionResult<bool> CreateTransfer(int fromUserId, int toUserId, decimal transferAmount)
        {
            bool isUpdated = _transferDAO.CreateTransfer(fromUserId, toUserId, transferAmount);

            if (isUpdated)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
