using Bernie.Server.Core;
using Bernie.Server.Model;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace Bernie.Server.Controllers
{
    [Authorize]
    public class SystemController : Controller
    {
        private readonly ISecuritySystem securitySystem;
        private readonly IRecentEventLog recentEvents;

        public SystemController(ISecuritySystem securitySystem, IRecentEventLog recentEvents)
        {
            this.securitySystem = securitySystem;
            this.recentEvents = recentEvents;
        }

        public IActionResult Index()
        {
            var model = new SystemModel
            {
                State = securitySystem.State,
                Events = recentEvents.GetRecentEvents()
            };

            return View(model);
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Disarm()
        {
            securitySystem.Disarm();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Arm()
        {
            securitySystem.Arm();

            return RedirectToAction("Index");
        }
    }
}
