using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Contexts;
using WebApplication1.Models.DBModels;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly MessengerDBContext _dbContext;

        public AccountController(MessengerDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            return View(new AccountViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Password = user.Password,
                Visible = user.Visible,
                Identifier = user.Identifier,
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _dbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

                if (user.UserName != model.UserName)
                {
                    if (!_dbContext.Users.Any(u => u.UserName == model.UserName))
                    {
                        user.UserName = model.UserName;
                    }
                    else
                    {
                        ModelState.AddModelError("", "This name is already taken!");
                    }
                }

                user.Visible = model.Visible;
                user.Email = model.Email;
                user.Password = model.Password;

                _dbContext.SaveChanges();
            }
            await Authenticate(model.UserName);
            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _dbContext.Users.FirstOrDefault(u =>
                    u.UserName == model.UserName
                    && u.Password == model.Password);

                if(user != null)
                {
                    await Authenticate(model.UserName);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Password or Email was entered incorrectly!");
            }

            return View(model);
        }

        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _dbContext.Users.FirstOrDefault(u => u.UserName == model.UserName);

                if(user is null)
                {
                    var rand = new Random(DateTime.Now.Millisecond);

                    _dbContext.Users.Add(new User
                    {
                        UserName = model.UserName,
                        Password = model.Password,
                        Email = model.Email,
                        Identifier = rand.Next(10000, 100000),
                        Visible = true,
                        Key = rand.Next(0, 100),
                    });

                    _dbContext.SaveChanges();

                    await Authenticate(model.UserName);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "User already exist!");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };

            var id = new ClaimsIdentity(claims,
                                        "ApplicationCookie",
                                        ClaimsIdentity.DefaultNameClaimType,
                                        ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                          new ClaimsPrincipal(id));
        }
    }
}
