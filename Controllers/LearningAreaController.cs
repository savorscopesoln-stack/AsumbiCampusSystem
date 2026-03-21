using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AsumbiCampusSystem.Models;
using AsumbiCampusSystem.Data;
using System.Collections.Generic;

namespace AsumbiCampusSystem.Controllers
{
    public class LearningAreaController : Controller
    {
        // ==========================
        // LIST LEARNING AREAS PER COURSE
        // ==========================
        public IActionResult Index(int courseId)
        {
            var course = InMemoryData.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null)
                return NotFound();

            var topics = InMemoryData.LearningAreas
                .Where(l => l.CourseId == courseId)
                .OrderBy(l => l.Id)
                .ToList();

            ViewBag.CourseName = course.CourseName;
            ViewBag.CourseId = course.Id;

            return View(topics);
        }

        // ==========================
        // CREATE LEARNING AREA (GET)
        // ==========================
        [HttpGet]
        public IActionResult Create(int courseId)
        {
            var course = InMemoryData.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null)
                return NotFound();

            ViewBag.CourseName = course.CourseName;
            ViewBag.CourseId = course.Id;

            return View();
        }

        // ==========================
        // CREATE LEARNING AREA (POST)
        // ==========================
        [HttpPost]
        public IActionResult Create(int courseId, string topicName, string description)
        {
            var course = InMemoryData.Courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(topicName))
            {
                TempData["Error"] = "Topic name cannot be empty.";
                return RedirectToAction("Create", new { courseId });
            }

            var newId = InMemoryData.LearningAreas.Count > 0 ? InMemoryData.LearningAreas.Max(l => l.Id) + 1 : 1;
            InMemoryData.LearningAreas.Add(new LearningArea
            {
                Id = newId,
                CourseId = courseId,
                TopicName = topicName,
                Description = description
            });

            TempData["Success"] = $"Topic '{topicName}' added successfully!";
            return RedirectToAction("Index", new { courseId });
        }

        // ==========================
        // EDIT LEARNING AREA (GET)
        // ==========================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var topic = InMemoryData.LearningAreas.FirstOrDefault(l => l.Id == id);
            if (topic == null)
                return NotFound();

            return View(topic);
        }

        // ==========================
        // EDIT LEARNING AREA (POST)
        // ==========================
        [HttpPost]
        public IActionResult Edit(int id, string topicName, string description)
        {
            var topic = InMemoryData.LearningAreas.FirstOrDefault(l => l.Id == id);
            if (topic == null)
                return NotFound();

            topic.TopicName = topicName;
            topic.Description = description;

            TempData["Success"] = "Topic updated successfully!";
            return RedirectToAction("Index", new { courseId = topic.CourseId });
        }

        // ==========================
        // DELETE LEARNING AREA
        // ==========================
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var topic = InMemoryData.LearningAreas.FirstOrDefault(l => l.Id == id);
            if (topic != null)
            {
                InMemoryData.LearningAreas.Remove(topic);
                TempData["Success"] = "Topic removed successfully!";
            }

            return RedirectToAction("Index", new { courseId = topic?.CourseId ?? 0 });
        }
    }
}