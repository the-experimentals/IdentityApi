using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace IdentityApi.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Testing account endpoint");
        }
    }
}
