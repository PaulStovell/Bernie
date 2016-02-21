using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bernie.Server.Core;
using Microsoft.AspNet.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Bernie.Server.Controllers
{
    [Route("api/[controller]")]
    public class SensorsController : Controller
    {
        private readonly ISecuritySystem securitySystem;

        public SensorsController(ISecuritySystem securitySystem)
        {
            this.securitySystem = securitySystem;
        }

        [HttpPost]
        public void Post([FromBody]string rule, [FromBody]string sensor)
        {
            securitySystem.MotionDetected();
        }
    }
}
