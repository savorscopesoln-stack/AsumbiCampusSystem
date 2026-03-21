using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AsumbiCampusSystem.Data;
using AsumbiCampusSystem.Models;
using AsumbiCampusSystem.Helpers;

namespace AsumbiCampusSystem.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContextNew _context;

        public UsersController(AppDbContextNew context)
        {
            _context = context;
        }

        private bool Allowed() => RoleHelper.HasAnyRole(HttpContext, "Master Admin");

        // GET: Users
        public async Task<IActionResult> Index()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var users = await _context.Users.Include(u => u.Student).ToListAsync();
            return View(users);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            ViewBag.Students = _context.Students.ToList();
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Password,Role,StudentId")] User user)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            if (string.IsNullOrWhiteSpace(user.Username) ||
                string.IsNullOrWhiteSpace(user.Password) ||
                string.IsNullOrWhiteSpace(user.Role))
            {
                ViewBag.Error = "Username, Password, and Role are required.";
                ViewBag.Students = _context.Students.ToList();
                return View(user);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            TempData["Success"] = "User added successfully!";
            return RedirectToAction("Index");
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewBag.Students = _context.Students.ToList();
            return View(user);
        }

        // POST: Users/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Username,Password,Role,StudentId")] User updatedUser)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var user = await _context.Users.FindAsync(updatedUser.Id);
            if (user == null) return NotFound();

            user.Username = updatedUser.Username;
            user.Password = updatedUser.Password;
            user.Role = updatedUser.Role;
            user.StudentId = updatedUser.StudentId;

            await _context.SaveChangesAsync();
            TempData["Success"] = "User updated successfully!";
            return RedirectToAction("Index");
        }

        // POST: Users/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "User deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}