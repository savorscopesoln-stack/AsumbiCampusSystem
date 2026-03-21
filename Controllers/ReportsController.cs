using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AsumbiCampusSystem.Data;
using AsumbiCampusSystem.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AsumbiCampusSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly AppDbContextNew _context;

        public ReportsController(AppDbContextNew context)
        {
            _context = context;
        }

        public async Task<IActionResult> Weekly()
        {
            if (!RoleHelper.HasAnyRole(HttpContext, "Master Admin", "Deputy Admin", "Teacher On Duty"))
                return RedirectToAction("AccessDenied", "Account");

            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1); // Monday
            var endOfWeek = startOfWeek.AddDays(6); // Sunday

            // Fetch weekly attendance
            var weeklyAttendance = await _context.AttendanceRecords
                .Where(a => a.Date.Date >= startOfWeek && a.Date.Date <= endOfWeek)
                .ToListAsync();

            // Fetch weekly leave records
            var weeklyLeaves = await _context.LeaveRecords
                .Where(l => l.TimeOut.Date >= startOfWeek && l.TimeOut.Date <= endOfWeek)
                .ToListAsync();

            // Fetch active students
            var totalStudents = await _context.Students.CountAsync();

            // Fetch staff
            var totalStaff = await _context.StaffMembers.CountAsync();

            ViewBag.WeeklyAttendance = weeklyAttendance;
            ViewBag.WeeklyLeaves = weeklyLeaves;
            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalStaff = totalStaff;
            ViewBag.StartOfWeek = startOfWeek.ToString("yyyy-MM-dd");
            ViewBag.EndOfWeek = endOfWeek.ToString("yyyy-MM-dd");

            return View();
        }
    }
}