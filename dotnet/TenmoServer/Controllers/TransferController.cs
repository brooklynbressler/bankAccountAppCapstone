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

        public TransferController(IUserDAO userDAO = null)
        {
            _userDAO = userDAO;
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

        [HttpGet("{username}")]
        public ActionResult<User> GetUser(string username)
        {
            User user = _userDAO.GetUser(username);

            if(user != null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
