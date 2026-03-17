using Microsoft.AspNetCore.Mvc;
using AsumbiCampusSystem.Models;
using AsumbiCampusSystem.Helpers;

namespace AsumbiCampusSystem.Controllers
{
    public class AttendanceController : Controller
    {
        private static List<AttendanceRecord> attendance = new List<AttendanceRecord>
        {
            new AttendanceRecord
            {
                Id = 1,
                StudentName = "John Otieno",
                AdmissionNumber = "ATC001",
                ClassName = "P1 Science",
                Date = "2026-03-14",
                Status = "Present",
                RecordedBy = "Mr. Onyango"
            }
        };

        private bool Allowed()
        {
            return RoleHelper.HasAnyRole(HttpContext, "Master Admin", "Deputy Admin", "Class Teacher");
        }

        public IActionResult Index()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");
            return View(attendance);
        }

        public IActionResult Details(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var record = attendance.FirstOrDefault(a => a.Id == id);
            if (record == null) return NotFound();
            return View(record);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");
            return View();
        }

        [HttpPost]
        public IActionResult Create(AttendanceRecord record)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            if (string.IsNullOrWhiteSpace(record.StudentName) ||
                string.IsNullOrWhiteSpace(record.AdmissionNumber) ||
                string.IsNullOrWhiteSpace(record.ClassName))
            {
                ViewBag.Error = "Student Name, Admission Number, and Class Name are required.";
                return View(record);
            }

            record.Id = attendance.Count == 0 ? 1 : attendance.Max(a => a.Id) + 1;
            if (string.IsNullOrWhiteSpace(record.Status)) record.Status = "Present";

            attendance.Add(record);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var record = attendance.FirstOrDefault(a => a.Id == id);
            if (record == null) return NotFound();
            return View(record);
        }

        [HttpPost]
        public IActionResult Edit(AttendanceRecord updatedRecord)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var record = attendance.FirstOrDefault(a => a.Id == updatedRecord.Id);
            if (record == null) return NotFound();

            record.StudentName = updatedRecord.StudentName;
            record.AdmissionNumber = updatedRecord.AdmissionNumber;
            record.ClassName = updatedRecord.ClassName;
            record.Date = updatedRecord.Date;
            record.Status = updatedRecord.Status;
            record.RecordedBy = updatedRecord.RecordedBy;

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var record = attendance.FirstOrDefault(a => a.Id == id);
            if (record == null) return NotFound();
            return View(record);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var record = attendance.FirstOrDefault(a => a.Id == id);
            if (record == null) return NotFound();

            attendance.Remove(record);
            return RedirectToAction("Index");
        }
    }
}