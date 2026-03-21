using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AsumbiCampusSystem.Data;
using AsumbiCampusSystem.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace AsumbiCampusSystem.Controllers
{
    [Authorize(Roles = "Kitchen Staff,Admin,Deputy,Master Admin")]
    public class KitchenStaffController : Controller
    {
        private readonly AppDbContextNew _context;

        public KitchenStaffController(AppDbContextNew context)
        {
            _context = context;
        }

        // GET: /KitchenStaff/Scan
        [HttpGet]
        public IActionResult Scan()
        {
            return View();
        }

        // POST: /KitchenStaff/Scan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Scan(string rfidUID, string mealType)
        {
            if (string.IsNullOrEmpty(rfidUID))
            {
                TempData["Message"] = "RFID UID is required.";
                return RedirectToAction("Scan");
            }

            // Get the logged-in staff Id from claims or session
            int staffId = 0;
            var staffIdClaim = User.Claims.FirstOrDefault(c => c.Type == "StaffId")?.Value;
            if (!string.IsNullOrEmpty(staffIdClaim))
            {
                int.TryParse(staffIdClaim, out staffId);
            }

            var student = _context.Students.FirstOrDefault(s => s.RFID_UID == rfidUID);

            if (student == null)
            {
                // Unknown student scanned
                var unknownMeal = new MealRecord
                {
                    StudentName = "Unknown",
                    AdmissionNumber = "Unknown",
                    MealType = mealType,
                    Status = "Denied",
                    AttemptedWithoutCard = true,
                    Date = DateTime.Now,
                    StaffId = staffId,
                    Remarks = "Unknown student scanned"
                };
                _context.MealRecords.Add(unknownMeal);
                _context.SaveChanges();

                TempData["Message"] = "Student not found. Meal Denied.";
            }
            else
            {
                // Check if student has an active meal card
                var validCard = _context.MealCards.Any(m =>
                    m.StudentId == student.Id &&
                    m.IsActive &&
                    m.StartDate <= DateTime.Now &&
                    m.EndDate >= DateTime.Now
                );

                var mealRecord = new MealRecord
                {
                    StudentId = student.Id,
                    StudentName = student.FullName,
                    AdmissionNumber = student.AdmissionNumber,
                    MealType = mealType,
                    Status = validCard ? "Served" : "Denied",
                    AttemptedWithoutCard = !validCard,
                    Date = DateTime.Now,
                    StaffId = staffId,
                    Remarks = validCard ? "Meal Served" : "Meal Denied - Invalid Card"
                };

                _context.MealRecords.Add(mealRecord);
                _context.SaveChanges();

                TempData["Message"] = validCard ? "Meal Served." : "Meal Denied - Invalid Card.";
            }

            return RedirectToAction("Scan");
        }

        // GET: /KitchenStaff/Dashboard
        public IActionResult Dashboard()
        {
            var meals = _context.MealRecords
                .OrderByDescending(m => m.Date)
                .ToList();
            return View(meals);
        }
    }
}