using Microsoft.AspNetCore.Mvc;
using AsumbiCampusSystem.Models;
using AsumbiCampusSystem.Data;
using System.Linq;

namespace AsumbiCampusSystem.Controllers
{
    public class CourseController : Controller
    {
        // ==========================
        // LIST COURSES
        // ==========================
        public IActionResult Index()
        {
            var courses = InMemoryData.Courses;
            return View(courses);
        }

        // ==========================
        // CREATE COURSE
        // ==========================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string courseName, string description, int teacherId)
        {
            var newId = InMemoryData.Courses.Count > 0 ? InMemoryData.Courses.Max(c => c.Id) + 1 : 1;

            var course = new Course
            {
                Id = newId,
                CourseName = courseName,
                Description = description,
                TeacherId = teacherId
            };

            InMemoryData.Courses.Add(course);
            TempData["Success"] = "Course created successfully!";
            return RedirectToAction("Index");
        }

        // ==========================
        // EDIT COURSE
        // ==========================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var course = InMemoryData.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return NotFound();
            return View(course);
        }

        [HttpPost]
        public IActionResult Edit(int id, string courseName, string description, int teacherId)
        {
            var course = InMemoryData.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return NotFound();

            course.CourseName = courseName;
            course.Description = description;
            course.TeacherId = teacherId;

            TempData["Success"] = "Course updated successfully!";
            return RedirectToAction("Index");
        }

        // ==========================
        // DELETE COURSE
        // ==========================
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var course = InMemoryData.Courses.FirstOrDefault(c => c.Id == id);
            if (course != null)
            {
                InMemoryData.Courses.Remove(course);
                TempData["Success"] = "Course deleted successfully!";
            }
            return RedirectToAction("Index");
        }

        // ==========================
        // DETAILS
        // ==========================
        public IActionResult Details(int id)
        {
            var course = InMemoryData.Courses.FirstOrDefault(c => c.Id == id);
            if (course == null) return NotFound();
            return View(course);
        }
    }
}