using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
 [Route("api/[controller]")]
    [Authorize]
    public class IdentityController : ControllerBase
    {

        public IdentityController()
        {
        }
        [HttpGet()]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}
