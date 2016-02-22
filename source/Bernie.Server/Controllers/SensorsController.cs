using System.Net;
using System.Net.Http;
using Bernie.Server.Core;
using Bernie.Server.Model;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;

namespace Bernie.Server.Controllers
{
    [Route("api/[controller]")]
    public class SensorsController : Controller
    {
        private readonly ISecuritySystem securitySystem;
        private readonly ISensorAuthenticator sensorAuthenticator;
        private readonly ILogger logger;

        public SensorsController(ISecuritySystem securitySystem, ISensorAuthenticator sensorAuthenticator, ILoggerFactory logger)
        {
            this.securitySystem = securitySystem;
            this.sensorAuthenticator = sensorAuthenticator;
            this.logger = logger.CreateLogger("Sensors API");
        }

        [HttpPost]
        public HttpResponseMessage Post([FromQuery]string token, [FromForm]string rule, [FromForm]string sensor)
        {
            if (!sensorAuthenticator.AuthenticateToken(token))
            {
                logger.LogWarning("Sensor update detected with invalid token");
                return new HttpResponseMessage(HttpStatusCode.Forbidden);
            }

            securitySystem.MotionDetected(sensor);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
