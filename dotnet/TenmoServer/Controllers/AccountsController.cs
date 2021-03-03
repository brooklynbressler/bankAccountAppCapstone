using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountDAO _dao;

        public AccountsController(IAccountDAO accountdao = null)
        {
                _dao = accountdao;
        }

        [HttpGet]
        public ActionResult<decimal> GetAccountBalance(int userId)
        {
            int id = Convert.ToInt32(User.FindFirst("sub")?.Value);
            Account account = _dao.GetAccount(id);

            if(account != null)
            {
                return Ok(account.Balance);
            }
            else
            {
                return NotFound();
            }
        }

    }
}
