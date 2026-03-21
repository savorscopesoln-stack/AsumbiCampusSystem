using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AsumbiCampusSystem.Data;
using AsumbiCampusSystem.Models;
using AsumbiCampusSystem.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AsumbiCampusSystem.Controllers
{
    public class LeaveController : Controller
    {
        private readonly AppDbContextNew _context;
        public LeaveController(AppDbContextNew context) { _context = context; }

        private bool Allowed() =>
            RoleHelper.HasAnyRole(HttpContext,
                "Master Admin",
                "Deputy Admin",
                "Teacher On Duty",
                "Dean of Students");

        private string GetUserRole() => HttpContext.Session.GetString("Role") ?? "";
        private string GetUserName() => HttpContext.Session.GetString("Username") ?? "";

        // Auto-update leave statuses (without GateStatus)
        private void UpdateStatuses()
        {
            var now = DateTime.Now;
            var leaves = _context.LeaveRecords.ToList();

            foreach (var leave in leaves)
            {
                // Pending and Denied remain unchanged
                if (leave.Status == "Pending" || leave.Status == "Denied") continue;

                if (leave.ActualReturn != null)
                    leave.Status = "Completed";
                else if (leave.TimeOut <= now && leave.ExpectedReturn >= now)
                    leave.Status = "Active";
                else if (now > leave.ExpectedReturn)
                    leave.Status = "Overdue";
            }

            _context.SaveChanges();
        }

        // GET: Index / Leaves Dashboard
        public async Task<IActionResult> Index()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            UpdateStatuses();
            var leaves = await _context.LeaveRecords.OrderByDescending(l => l.TimeOut).ToListAsync();
            return View(leaves);
        }

        // GET: Create Leave
        [HttpGet]
        public IActionResult Create()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");
            return View();
        }

        // POST: Create Leave
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveRecord leave)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var role = GetUserRole();
            var durationHours = (leave.ExpectedReturn - leave.TimeOut).TotalHours;

            if (role == "Teacher On Duty" && durationHours > 12)
            {
                ModelState.AddModelError("", "Cannot give an overnight leave. TOD can only approve up to 12 hours.");
                return View(leave);
            }

            if (leave.ExpectedReturn <= leave.TimeOut)
            {
                ModelState.AddModelError("", "Return date must be after Time Out.");
                return View(leave);
            }

            var student = await _context.Students.FirstOrDefaultAsync(s => s.AdmissionNumber == leave.AdmissionNumber);
            if (student == null)
            {
                ModelState.AddModelError("", "Student not found!");
                return View(leave);
            }

            // Auto-fill details
            leave.StudentName = student.FullName;
            leave.ClassName = student.ClassName;
            leave.Status = "Pending";
            leave.LeaveType = "Normal";
            leave.GateStatus = ""; // default

            _context.LeaveRecords.Add(leave);
            await _context.SaveChangesAsync();

            // Notify next approver
            string nextRole = (leave.ExpectedReturn - leave.TimeOut).TotalDays <= 14 ? "Dean of Students" : "Deputy Admin";
            var approvers = _context.Users.Where(u => u.Role == nextRole).ToList();
            foreach (var approver in approvers)
            {
                NotificationHelper.Send(_context, approver.Id, "Leave Approval Required",
                    $"{leave.StudentName} requested leave from {leave.TimeOut:yyyy-MM-dd HH:mm} to {leave.ExpectedReturn:yyyy-MM-dd HH:mm}");
            }

            TempData["SuccessMessage"] = "Leave request created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Approve Leave (AJAX)
        [HttpPost]
        public async Task<JsonResult> Approve(int id)
        {
            var leave = await _context.LeaveRecords.FindAsync(id);
            if (leave == null)
                return Json(new { success = false, message = "Leave not found." });

            if (leave.Status != "Pending")
                return Json(new { success = false, message = "Leave already processed." });

            leave.Status = "Approved";
            leave.ApprovedBy = GetUserName();
            leave.ApprovedByRole = GetUserRole();
            _context.LeaveRecords.Update(leave);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // POST: Delete Leave (AJAX)
        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var leave = await _context.LeaveRecords.FindAsync(id);
            if (leave == null)
                return Json(new { success = false, message = "Leave not found." });

            _context.LeaveRecords.Remove(leave);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // GET: fetch student info for auto-fill
        [HttpGet]
        public async Task<JsonResult> GetStudentByAdmission(string admissionNumber)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.AdmissionNumber == admissionNumber);
            if (student == null) return Json(null);

            return Json(new { fullName = student.FullName, className = student.ClassName });
        }
    }
}