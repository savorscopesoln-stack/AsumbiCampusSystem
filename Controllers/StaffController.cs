using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AsumbiCampusSystem.Models;
using AsumbiCampusSystem.Data;
using AsumbiCampusSystem.Helpers;

namespace AsumbiCampusSystem.Controllers
{
    public class StaffController : Controller
    {
        private readonly AppDbContextNew _context;

        public StaffController(AppDbContextNew context)
        {
            _context = context;
        }

        private bool Allowed()
        {
            return RoleHelper.HasAnyRole(HttpContext, "Master Admin", "Deputy Admin");
        }

        // GET: Staff
        public async Task<IActionResult> Index()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");
            var staff = await _context.StaffMembers.ToListAsync();
            return View(staff);
        }

        // GET: Staff/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var member = await _context.StaffMembers.FindAsync(id);
            if (member == null) return NotFound();
            return View(member);
        }

        // GET: Staff/Create
        [HttpGet]
        public IActionResult Create()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");
            return View();
        }

        // POST: Staff/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StaffMember member)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            if (string.IsNullOrWhiteSpace(member.FullName) ||
                string.IsNullOrWhiteSpace(member.StaffNumber) ||
                string.IsNullOrWhiteSpace(member.Department))
            {
                ViewBag.Error = "Full Name, Staff Number, and Department are required.";
                return View(member);
            }

            if (string.IsNullOrWhiteSpace(member.Status)) member.Status = "Active";

            _context.StaffMembers.Add(member);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Staff/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var member = await _context.StaffMembers.FindAsync(id);
            if (member == null) return NotFound();
            return View(member);
        }

        // POST: Staff/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StaffMember updatedMember)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            if (id != updatedMember.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(updatedMember);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(updatedMember);
        }

        // GET: Staff/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var member = await _context.StaffMembers.FindAsync(id);
            if (member == null) return NotFound();
            return View(member);
        }

        // POST: Staff/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var member = await _context.StaffMembers.FindAsync(id);
            if (member == null) return NotFound();

            _context.StaffMembers.Remove(member);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}