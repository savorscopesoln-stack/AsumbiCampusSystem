using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AsumbiCampusSystem.Models;
using AsumbiCampusSystem.Data;
using AsumbiCampusSystem.Helpers;

namespace AsumbiCampusSystem.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly AppDbContextNew _context;

        public AttendanceController(AppDbContextNew context)
        {
            _context = context;
        }

        private bool Allowed()
        {
            return RoleHelper.HasAnyRole(HttpContext, "Master Admin", "Deputy Admin", "Class Teacher");
        }

        // GET: Attendance
        public async Task<IActionResult> Index()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var attendance = await _context.AttendanceRecords.ToListAsync();
            return View(attendance);
        }

        // GET: Attendance/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var record = await _context.AttendanceRecords.FindAsync(id);
            if (record == null) return NotFound();

            return View(record);
        }

        // GET: Attendance/Create
        public IActionResult Create()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");
            return View();
        }

        // POST: Attendance/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentName,ClassName,Date,Status")] AttendanceRecord attendance)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            if (attendance.Date == default) attendance.Date = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.AttendanceRecords.Add(attendance);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Attendance recorded successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(attendance);
        }

        // GET: Attendance/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var record = await _context.AttendanceRecords.FindAsync(id);
            if (record == null) return NotFound();

            return View(record);
        }

        // POST: Attendance/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentName,ClassName,Date,Status")] AttendanceRecord updatedRecord)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            if (id != updatedRecord.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(updatedRecord);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Attendance updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendanceRecordExists(updatedRecord.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(updatedRecord);
        }

        // GET: Attendance/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var record = await _context.AttendanceRecords.FindAsync(id);
            if (record == null) return NotFound();

            return View(record);
        }

        // POST: Attendance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var record = await _context.AttendanceRecords.FindAsync(id);
            if (record != null)
            {
                _context.AttendanceRecords.Remove(record);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Attendance deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AttendanceRecordExists(int id)
        {
            return _context.AttendanceRecords.Any(e => e.Id == id);
        }
    }
}