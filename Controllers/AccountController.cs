using Microsoft.AspNetCore.Mvc;
using AsumbiCampusSystem.Models;

namespace AsumbiCampusSystem.Controllers
{
    public class AccountController : Controller
    {
        private static readonly List<(string Username, string Password, string Role)> users = new()
        {
            ("masteradmin", "1234", "Master Admin"),
            ("deputyadmin", "1234", "Deputy Admin"),
            ("classteacher", "1234", "Class Teacher"),
            ("tod", "1234", "Teacher On Duty"),
            ("gatestaff", "1234", "Gate Staff"),
            ("kitchenstaff", "1234", "Kitchen Staff")
        };

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            var user = users.FirstOrDefault(u =>
                u.Username.Equals(model.Username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == model.Password);

            if (string.IsNullOrWhiteSpace(user.Username))
            {
                ViewBag.Error = "Invalid username or password.";
                return View(model);
            }

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);

            if (user.Role == "Master Admin" || user.Role == "Deputy Admin")
                return RedirectToAction("Index", "Dashboard");

            if (user.Role == "Class Teacher")
                return RedirectToAction("Index", "Attendance");

            if (user.Role == "Teacher On Duty")
                return RedirectToAction("Index", "Leave");

            if (user.Role == "Kitchen Staff")
                return RedirectToAction("Index", "Meals");

            if (user.Role == "Gate Staff")
                return RedirectToAction("Index", "Home");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}