using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LoginReg.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace LoginReg.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        public HomeController(MyContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return RedirectToAction("RegisterForm");
        }
        [HttpGet("register")]
        public IActionResult RegisterForm()
        {
            return View();
        }
        [HttpPost("new/user")]
        public IActionResult NewUser(User newUser)
        {
            if (ModelState.IsValid)
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                _context.Add(newUser);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("UserId", newUser.UserId);
                return RedirectToAction("Success");
            }
            return View("RegisterForm");
        }
        [HttpGet("success")]
        public IActionResult Success()
        {
            int? loggedId = HttpContext.Session.GetInt32("UserId");
            if(loggedId ==null)
            {
                return RedirectToAction("RegisterForm");
            }
            User LoggedUser = _context.Users.FirstOrDefault(user => user.UserId == (int)loggedId);
            ViewBag.loggedUser = LoggedUser;
            return View();
        }
        [HttpGet("loginPage")]
        public IActionResult LoginPage()
        {
            return View();
        }
        [HttpPost("login")]
        public IActionResult Login(LoginUser LogUser)
        {
            Console.WriteLine("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
            if(ModelState.IsValid)
            {
                var dbUser = _context.Users.FirstOrDefault(user => user.Email == LogUser.Email);
                if(dbUser == null)
                {
                    ModelState.AddModelError("Email", "Invalid login");
                    return View("LoginPage");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(LogUser, dbUser.Password, LogUser.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid login");
                    return View("LoginPage");
                }
                HttpContext.Session.SetInt32("UserId", dbUser.UserId);
                return RedirectToAction("Success");
            }
            return View("LoginPage");
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginPage");
        }
    }
}
