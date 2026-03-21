using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AsumbiCampusSystem.Models;
using AsumbiCampusSystem.Data;
using Rotativa.AspNetCore; // For PDF download
using System;

namespace AsumbiCampusSystem.Controllers
{
    public class ResultController : Controller
    {
        // ==========================
        // VIEW RESULTS (STUDENT)
        // ==========================
        public IActionResult Index(int studentId)
        {
            // Fetch results for the student
            var results = InMemoryData.Results
                .Where(r => r.StudentId == studentId)
                .Select(r => new ResultViewModel
                {
                    Course = InMemoryData.Courses.FirstOrDefault(c => c.Id == r.CourseId)?.CourseName,
                    Topic = InMemoryData.LearningAreas.FirstOrDefault(l => l.Id == r.LearningAreaId)?.TopicName,
                    CATName = r.CATName,
                    Score = r.Score,
                    DateRecorded = r.DateRecorded
                }).ToList();

            ViewBag.Student = InMemoryData.Students.FirstOrDefault(s => s.Id == studentId)?.FullName;
            ViewBag.StudentId = studentId;

            return View(results);
        }

        // ==========================
        // ADD RESULT (TEACHER)
        // ==========================
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Students = InMemoryData.Students;
            ViewBag.Courses = InMemoryData.Courses;
            ViewBag.Topics = InMemoryData.LearningAreas;
            return View();
        }

        [HttpPost]
        public IActionResult Create(int studentId, int courseId, int learningAreaId, string catName, double score)
        {
            var newId = InMemoryData.Results.Count > 0 ? InMemoryData.Results.Max(r => r.Id) + 1 : 1;

            InMemoryData.Results.Add(new Result
            {
                Id = newId,
                StudentId = studentId,
                CourseId = courseId,
                LearningAreaId = learningAreaId,
                CATName = catName,
                Score = score,
                DateRecorded = DateTime.Now
            });

            TempData["Success"] = "Marks recorded successfully!";
            return RedirectToAction("Index", new { studentId });
        }

        // ==========================
        // DOWNLOAD RESULTS AS PDF
        // ==========================
        public IActionResult Download(int studentId)
        {
            var results = InMemoryData.Results
                .Where(r => r.StudentId == studentId)
                .Select(r => new ResultViewModel
                {
                    Course = InMemoryData.Courses.FirstOrDefault(c => c.Id == r.CourseId)?.CourseName,
                    Topic = InMemoryData.LearningAreas.FirstOrDefault(l => l.Id == r.LearningAreaId)?.TopicName,
                    CATName = r.CATName,
                    Score = r.Score,
                    DateRecorded = r.DateRecorded
                }).ToList();

            var student = InMemoryData.Students.FirstOrDefault(s => s.Id == studentId);

            if (student == null)
            {
                TempData["Error"] = "Student not found.";
                return RedirectToAction("Index", "Home");
            }

            // Ensure view exists at /Views/Result/PdfTemplates/StudentResult.cshtml
            return new ViewAsPdf("PdfTemplates/StudentResult", results)
            {
                FileName = $"{student.FullName}_Results.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }
    }

    // ==========================
    // HELPER VIEWMODEL FOR RESULTS
    // ==========================
    public class ResultViewModel
    {
        public string Course { get; set; }
        public string Topic { get; set; }
        public string CATName { get; set; }
        public double Score { get; set; }
        public DateTime? DateRecorded { get; set; }
    }
}