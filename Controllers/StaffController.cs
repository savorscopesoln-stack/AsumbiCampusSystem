using Microsoft.AspNetCore.Mvc;
using AsumbiCampusSystem.Models;
using AsumbiCampusSystem.Helpers;

namespace AsumbiCampusSystem.Controllers
{
    public class StaffController : Controller
    {
        private static List<StaffMember> staff = new List<StaffMember>
        {
            new StaffMember
            {
                Id = 1,
                FullName = "Mr. Onyango",
                StaffNumber = "ST001",
                Department = "Education",
                PhoneNumber = "0712345678",
                RFID_UID = "STAFF001",
                Status = "Active"
            },
            new StaffMember
            {
                Id = 2,
                FullName = "Ms. Akinyi",
                StaffNumber = "ST002",
                Department = "ICT",
                PhoneNumber = "0798765432",
                RFID_UID = "STAFF002",
                Status = "Active"
            }
        };

        private bool Allowed()
        {
            return RoleHelper.HasAnyRole(HttpContext, "Master Admin", "Deputy Admin");
        }

        public IActionResult Index()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");
            return View(staff);
        }

        public IActionResult Details(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var member = staff.FirstOrDefault(s => s.Id == id);
            if (member == null) return NotFound();
            return View(member);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");
            return View();
        }

        [HttpPost]
        public IActionResult Create(StaffMember member)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            if (string.IsNullOrWhiteSpace(member.FullName) ||
                string.IsNullOrWhiteSpace(member.StaffNumber) ||
                string.IsNullOrWhiteSpace(member.Department))
            {
                ViewBag.Error = "Full Name, Staff Number, and Department are required.";
                return View(member);
            }

            member.Id = staff.Count == 0 ? 1 : staff.Max(s => s.Id) + 1;
            if (string.IsNullOrWhiteSpace(member.Status)) member.Status = "Active";

            staff.Add(member);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var member = staff.FirstOrDefault(s => s.Id == id);
            if (member == null) return NotFound();
            return View(member);
        }

        [HttpPost]
        public IActionResult Edit(StaffMember updatedMember)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var member = staff.FirstOrDefault(s => s.Id == updatedMember.Id);
            if (member == null) return NotFound();

            member.FullName = updatedMember.FullName;
            member.StaffNumber = updatedMember.StaffNumber;
            member.Department = updatedMember.Department;
            member.PhoneNumber = updatedMember.PhoneNumber;
            member.RFID_UID = updatedMember.RFID_UID;
            member.Status = updatedMember.Status;

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var member = staff.FirstOrDefault(s => s.Id == id);
            if (member == null) return NotFound();
            return View(member);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var member = staff.FirstOrDefault(s => s.Id == id);
            if (member == null) return NotFound();

            staff.Remove(member);
            return RedirectToAction("Index");
        }
    }
}