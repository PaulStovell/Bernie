using System.Net;
using System.Net.Http;
using Bernie.Server.Core;
using Bernie.Server.Model;
using Microsoft.AspNet.Mvc;

namespace Bernie.Server.Controllers
{
    [Route("api/[controller]")]
    public class SensorsController : Controller
    {
        private readonly ISecuritySystem securitySystem;
        private readonly ISensorAuthenticator sensorAuthenticator;

        public SensorsController(ISecuritySystem securitySystem, ISensorAuthenticator sensorAuthenticator)
        {
            this.securitySystem = securitySystem;
            this.sensorAuthenticator = sensorAuthenticator;
        }

        [HttpPost]
        public HttpResponseMessage Post([FromQuery]string token, [FromForm]string rule, [FromForm]string sensor)
        {
            if (!sensorAuthenticator.AuthenticateToken(token))
                return new HttpResponseMessage(HttpStatusCode.Forbidden);

            securitySystem.MotionDetected(sensor);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
