using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AsumbiCampusSystem.Models;
using AsumbiCampusSystem.Data;
using System.Collections.Generic;

namespace AsumbiCampusSystem.Controllers
{
    public class EnrollmentController : Controller
    {
        // ==========================
        // LIST ENROLLMENTS
        // ==========================
        public IActionResult Index()
        {
            // Combine Enrollment with Student and Course info
            var enrollmentList = InMemoryData.Enrollments.Select(e => new EnrollmentViewModel
            {
                Id = e.Id,
                StudentId = e.StudentId,
                StudentName = InMemoryData.Students.FirstOrDefault(s => s.Id == e.StudentId)?.FullName ?? "Unknown",
                CourseId = e.CourseId,
                CourseName = InMemoryData.Courses.FirstOrDefault(c => c.Id == e.CourseId)?.CourseName ?? "Unknown",
                EnrolledOn = e.EnrolledOn
            }).ToList();

            return View(enrollmentList);
        }

        // ==========================
        // CREATE ENROLLMENT (GET)
        // ==========================
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Students = InMemoryData.Students;
            ViewBag.Courses = InMemoryData.Courses;
            return View();
        }

        // ==========================
        // CREATE ENROLLMENT (POST)
        // ==========================
        [HttpPost]
        public IActionResult Create(int studentId, int courseId)
        {
            // Validate IDs
            var student = InMemoryData.Students.FirstOrDefault(s => s.Id == studentId);
            var course = InMemoryData.Courses.FirstOrDefault(c => c.Id == courseId);

            if (student == null || course == null)
            {
                TempData["Error"] = "Invalid Student or Course selection.";
                return RedirectToAction("Create");
            }

            // Check if already enrolled
            var exists = InMemoryData.Enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId);
            if (exists)
            {
                TempData["Error"] = $"{student.FullName} is already enrolled in {course.CourseName}.";
                return RedirectToAction("Create");
            }

            // Add enrollment
            var newId = InMemoryData.Enrollments.Count > 0 ? InMemoryData.Enrollments.Max(e => e.Id) + 1 : 1;
            InMemoryData.Enrollments.Add(new Enrollment
            {
                Id = newId,
                StudentId = studentId,
                CourseId = courseId,
                EnrolledOn = System.DateTime.Now
            });

            TempData["Success"] = $"{student.FullName} enrolled in {course.CourseName} successfully!";
            return RedirectToAction("Index");
        }

        // ==========================
        // DELETE ENROLLMENT
        // ==========================
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var enrollment = InMemoryData.Enrollments.FirstOrDefault(e => e.Id == id);
            if (enrollment != null)
            {
                InMemoryData.Enrollments.Remove(enrollment);
                TempData["Success"] = "Enrollment removed successfully!";
            }
            else
            {
                TempData["Error"] = "Enrollment not found.";
            }
            return RedirectToAction("Index");
        }
    }

    // ==========================
    // VIEW MODEL FOR ENROLLMENTS
    // ==========================
    public class EnrollmentViewModel
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = "";
        public int CourseId { get; set; }
        public string CourseName { get; set; } = "";
        public System.DateTime EnrolledOn { get; set; }
    }
}