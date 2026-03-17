using Microsoft.AspNetCore.Mvc;
using AsumbiCampusSystem.Models;
using AsumbiCampusSystem.Helpers;

namespace AsumbiCampusSystem.Controllers
{
    public class LeaveController : Controller
    {
        private static List<LeaveRecord> leaves = new List<LeaveRecord>
        {
            new LeaveRecord
            {
                Id = 1,
                StudentName = "John Otieno",
                AdmissionNumber = "ATC001",
                Reason = "Hospital Visit",
                TimeOut = "09:00 AM",
                ExpectedReturn = "04:00 PM",
                ActualReturn = "",
                Status = "Approved",
                ApprovedBy = "Mr. Onyango"
            }
        };

        private bool Allowed()
        {
            return RoleHelper.HasAnyRole(HttpContext, "Master Admin", "Deputy Admin", "Class Teacher", "Teacher On Duty");
        }

        public IActionResult Index()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");
            return View(leaves);
        }

        public IActionResult Details(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var leave = leaves.FirstOrDefault(l => l.Id == id);
            if (leave == null) return NotFound();
            return View(leave);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");
            return View();
        }

        [HttpPost]
        public IActionResult Create(LeaveRecord leave)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            if (string.IsNullOrWhiteSpace(leave.StudentName) ||
                string.IsNullOrWhiteSpace(leave.AdmissionNumber) ||
                string.IsNullOrWhiteSpace(leave.Reason))
            {
                ViewBag.Error = "Student Name, Admission Number, and Reason are required.";
                return View(leave);
            }

            leave.Id = leaves.Count == 0 ? 1 : leaves.Max(l => l.Id) + 1;
            if (string.IsNullOrWhiteSpace(leave.Status)) leave.Status = "Pending";

            leaves.Add(leave);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var leave = leaves.FirstOrDefault(l => l.Id == id);
            if (leave == null) return NotFound();
            return View(leave);
        }

        [HttpPost]
        public IActionResult Edit(LeaveRecord updatedLeave)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var leave = leaves.FirstOrDefault(l => l.Id == updatedLeave.Id);
            if (leave == null) return NotFound();

            leave.StudentName = updatedLeave.StudentName;
            leave.AdmissionNumber = updatedLeave.AdmissionNumber;
            leave.Reason = updatedLeave.Reason;
            leave.TimeOut = updatedLeave.TimeOut;
            leave.ExpectedReturn = updatedLeave.ExpectedReturn;
            leave.ActualReturn = updatedLeave.ActualReturn;
            leave.Status = updatedLeave.Status;
            leave.ApprovedBy = updatedLeave.ApprovedBy;

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var leave = leaves.FirstOrDefault(l => l.Id == id);
            if (leave == null) return NotFound();
            return View(leave);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var leave = leaves.FirstOrDefault(l => l.Id == id);
            if (leave == null) return NotFound();

            leaves.Remove(leave);
            return RedirectToAction("Index");
        }
    }
}