using Microsoft.AspNetCore.Mvc;
using AsumbiCampusSystem.Data;
using AsumbiCampusSystem.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace AsumbiCampusSystem.Controllers
{
    public class MealCardController : Controller
    {
        private readonly AppDbContextNew _context;

        public MealCardController(AppDbContextNew context)
        {
            _context = context;
        }

        // GET: Admin page to issue meal cards
        public IActionResult Issue()
        {
            var students = _context.Students.ToList();
            return View(students);
        }

        // POST: Issue meal cards to selected students
        [HttpPost]
        public IActionResult Issue(List<int> studentIds, int validityDays = 30)
        {
            var today = DateTime.Now;
            foreach(var id in studentIds)
            {
                var mealCard = new MealCard
                {
                    StudentId = id,
                    StartDate = today,
                    EndDate = today.AddDays(validityDays),
                    IsActive = true,
                    IssuedBy = User.Identity.Name
                };
                _context.MealCards.Add(mealCard);
            }
            _context.SaveChanges();
            return RedirectToAction("Issue"); // reload page
        }
    }
}