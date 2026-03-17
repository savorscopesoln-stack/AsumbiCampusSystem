using Microsoft.AspNetCore.Mvc;
using AsumbiCampusSystem.Models;
using AsumbiCampusSystem.Helpers;

namespace AsumbiCampusSystem.Controllers
{
    public class StudentsController : Controller
    {
        private static List<Student> students = new List<Student>
        {
            new Student
            {
                Id = 1,
                FullName = "John Otieno",
                AdmissionNumber = "ATC001",
                ClassName = "P1 Science",
                ParentPhone = "0712345678",
                RFID_UID = "A7F34C92",
                Status = "Active"
            },
            new Student
            {
                Id = 2,
                FullName = "Mary Achieng",
                AdmissionNumber = "ATC002",
                ClassName = "P1 Arts",
                ParentPhone = "0798765432",
                RFID_UID = "B234AC10",
                Status = "Active"
            }
        };

        private bool Allowed()
        {
            return RoleHelper.HasAnyRole(HttpContext, "Master Admin", "Deputy Admin", "Class Teacher");
        }

        public IActionResult Index()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");
            return View(students);
        }

        public IActionResult Details(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var student = students.FirstOrDefault(s => s.Id == id);
            if (student == null) return NotFound();
            return View(student);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Student student)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            if (string.IsNullOrWhiteSpace(student.FullName) ||
                string.IsNullOrWhiteSpace(student.AdmissionNumber) ||
                string.IsNullOrWhiteSpace(student.ClassName))
            {
                ViewBag.Error = "Full Name, Admission Number, and Class Name are required.";
                return View(student);
            }

            bool admissionExists = students.Any(s =>
                s.AdmissionNumber.Equals(student.AdmissionNumber, StringComparison.OrdinalIgnoreCase));

            if (admissionExists)
            {
                ViewBag.Error = "Admission number already exists.";
                return View(student);
            }

            student.Id = students.Count == 0 ? 1 : students.Max(s => s.Id) + 1;
            if (string.IsNullOrWhiteSpace(student.Status)) student.Status = "Active";

            students.Add(student);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var student = students.FirstOrDefault(s => s.Id == id);
            if (student == null) return NotFound();
            return View(student);
        }

        [HttpPost]
        public IActionResult Edit(Student updatedStudent)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var student = students.FirstOrDefault(s => s.Id == updatedStudent.Id);
            if (student == null) return NotFound();

            student.FullName = updatedStudent.FullName;
            student.AdmissionNumber = updatedStudent.AdmissionNumber;
            student.ClassName = updatedStudent.ClassName;
            student.ParentPhone = updatedStudent.ParentPhone;
            student.RFID_UID = updatedStudent.RFID_UID;
            student.Status = updatedStudent.Status;

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var student = students.FirstOrDefault(s => s.Id == id);
            if (student == null) return NotFound();
            return View(student);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var student = students.FirstOrDefault(s => s.Id == id);
            if (student == null) return NotFound();

            students.Remove(student);
            return RedirectToAction("Index");
        }
    }
}