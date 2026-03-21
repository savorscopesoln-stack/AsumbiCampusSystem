using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AsumbiCampusSystem.Models;
using AsumbiCampusSystem.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace AsumbiCampusSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContextNew _context;

        public AccountController(AppDbContextNew context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login() => View();

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username.ToLower() == model.Username.ToLower() &&
                    u.Password == model.Password);

            if (user == null)
            {
                ViewBag.Error = "Invalid username or password.";
                return View(model);
            }

            // ✅ Set session
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);

            if (user.Role == "Student" && user.StudentId.HasValue)
                HttpContext.Session.SetInt32("StudentId", user.StudentId.Value);

            if (user.Role == "Kitchen Staff" || user.Role == "Gate Staff")
                HttpContext.Session.SetInt32("StaffId", user.Id);

            // ✅ Cookie authentication
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("StaffId", user.Id.ToString()) // for kitchen/gate staff
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // -------------------------
            // Redirect based on role
            // -------------------------
            return user.Role switch
            {
                "Student" when user.StudentId.HasValue => RedirectToAction("Index", "Result", new { studentId = user.StudentId }),
                "Teacher" => RedirectToAction("Create", "Result"),
                "Class Teacher" => RedirectToAction("Create", "Result"),
                "Master Admin" => RedirectToAction("Index", "Dashboard"),
                "Deputy Admin" => RedirectToAction("Index", "Dashboard"),
                "Teacher On Duty" => RedirectToAction("Index", "Leave"),
                "Kitchen Staff" => RedirectToAction("Scan", "KitchenStaff"), // ✅ goes to Scan page
                "Gate Staff" => RedirectToAction("Scan", "GateStaff"),      // ✅ Gate Staff page
                _ => RedirectToAction("Index", "Home")
            };
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // GET: /Account/AccessDenied
        public IActionResult AccessDenied() => View();
    }
}