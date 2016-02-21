using System.Security.Claims;
using System.Threading.Tasks;
using Bernie.Server.Model;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Mvc;

namespace Bernie.Server.Controllers
{
    [AllowAnonymous]
    public class UserController : Controller
    {
        private readonly IUserAuthenticator userAuthenticator;

        public UserController(IUserAuthenticator userAuthenticator)
        {
            this.userAuthenticator = userAuthenticator;
        }

        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (!userAuthenticator.Authenticate(username, password))
            {
                ModelState.AddModelError("Password", "Invalid username or password");
                return View();
            }

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            }, "BernieCookie"));

            await HttpContext.Authentication.SignInAsync("BernieCookie", claimsPrincipal, new AuthenticationProperties
            {
                IsPersistent = true
            });

            return RedirectToAction("Index", "System");
        }
    }
}