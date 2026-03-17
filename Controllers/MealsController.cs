using Microsoft.AspNetCore.Mvc;
using AsumbiCampusSystem.Models;
using AsumbiCampusSystem.Helpers;

namespace AsumbiCampusSystem.Controllers
{
    public class MealsController : Controller
    {
        private static List<MealRecord> meals = new List<MealRecord>
        {
            new MealRecord
            {
                Id = 1,
                StudentName = "Mary Achieng",
                AdmissionNumber = "ATC002",
                MealType = "Lunch",
                Date = "2026-03-14",
                Status = "Served"
            }
        };

        private bool Allowed()
        {
            return RoleHelper.HasAnyRole(HttpContext, "Master Admin", "Deputy Admin", "Kitchen Staff");
        }

        public IActionResult Index()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");
            return View(meals);
        }

        public IActionResult Details(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var meal = meals.FirstOrDefault(m => m.Id == id);
            if (meal == null) return NotFound();
            return View(meal);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");
            return View();
        }

        [HttpPost]
        public IActionResult Create(MealRecord meal)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            if (string.IsNullOrWhiteSpace(meal.StudentName) ||
                string.IsNullOrWhiteSpace(meal.AdmissionNumber) ||
                string.IsNullOrWhiteSpace(meal.MealType))
            {
                ViewBag.Error = "Student Name, Admission Number, and Meal Type are required.";
                return View(meal);
            }

            meal.Id = meals.Count == 0 ? 1 : meals.Max(m => m.Id) + 1;
            if (string.IsNullOrWhiteSpace(meal.Status)) meal.Status = "Served";

            meals.Add(meal);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var meal = meals.FirstOrDefault(m => m.Id == id);
            if (meal == null) return NotFound();
            return View(meal);
        }

        [HttpPost]
        public IActionResult Edit(MealRecord updatedMeal)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var meal = meals.FirstOrDefault(m => m.Id == updatedMeal.Id);
            if (meal == null) return NotFound();

            meal.StudentName = updatedMeal.StudentName;
            meal.AdmissionNumber = updatedMeal.AdmissionNumber;
            meal.MealType = updatedMeal.MealType;
            meal.Date = updatedMeal.Date;
            meal.Status = updatedMeal.Status;

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var meal = meals.FirstOrDefault(m => m.Id == id);
            if (meal == null) return NotFound();
            return View(meal);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var meal = meals.FirstOrDefault(m => m.Id == id);
            if (meal == null) return NotFound();

            meals.Remove(meal);
            return RedirectToAction("Index");
        }
    }
}