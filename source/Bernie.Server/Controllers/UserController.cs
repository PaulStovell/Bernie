using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Mvc;

namespace Bernie.Server.Controllers
{
    [AllowAnonymous]
    public class UserController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string passcode)
        {
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "Known User")
            }, "BernieCookie"));

            await HttpContext.Authentication.SignInAsync("BernieCookie", claimsPrincipal, new AuthenticationProperties
            {
                IsPersistent = true
            });

            return RedirectToAction("Index", "System");
        }
    }
}