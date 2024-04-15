using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PasechnikovaPR33p19.Domain.Entities;
using PasechnikovaPR33p19.Domain.Services;
using PasechnikovaPR33p19.ViewModels;
using System.Security.Claims;
using System.Security.Principal;

namespace PasechnikovaPR33p19.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            try
            {
                await userService.GetUserAsync(login.Username, login.Password);
                User user = new User();
                user.Login = login.Username;
                user.Password = login.Password;
                return (IActionResult)SignIn(user);
            }
            catch
            {
                ModelState.AddModelError("reg_error", $"Неверное имя пользователя или пароль");
                return View(login);
            }
        }
        private async Task SignIn(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("fullname", user.Fullname),
                new Claim("id", user.Id.ToString()),
                new Claim("role", "client"),
                new Claim("username", user.Login)
            };
            string authType = CookieAuthenticationDefaults.AuthenticationScheme;
            IIdentity identity = new ClaimsIdentity(claims, authType, "username", "role");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);
        }
        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationViewModel registration)
        {
            if (!ModelState.IsValid)
            {
                return View(registration);
            }

            if (await userService.IsUserExistsAsync(registration.Username))
            {
                ModelState.AddModelError("user_exists", $"Имя пользователя {registration.Username} уже существует!");
                return View(registration);
            }


            try
            {
                await userService.RegistrationAsync(registration.Fullname, registration.Username, registration.Password);
                return RedirectToAction("RegistrationSuccess", "User");
            }
            catch
            {
                ModelState.AddModelError("reg_error", $"Не удалось зарегистрироваться, попробуйте попытку регистрации позже");
                return View(registration);
            }
        }
        public IActionResult RegistrationSuccess()
        {
            return View();
        }
        public IActionResult LoginSuccess()
        {
            return View();
        }
    }
}
