using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AsumbiCampusSystem.Data;
using AsumbiCampusSystem.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace AsumbiCampusSystem.Controllers
{
    // ✅ Include Kitchen Staff here
    [Authorize(Roles = "Admin,Deputy,Master Admin,Kitchen Staff")]
    public class MealsController : Controller
    {
        private readonly AppDbContextNew _context;

        public MealsController(AppDbContextNew context)
        {
            _context = context;

            // EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        // GET: Meals
        public async Task<IActionResult> Index()
        {
            var mealCards = await _context.MealCards
                .Include(m => m.Student)
                .OrderByDescending(m => m.StartDate)
                .ToListAsync();

            return View(mealCards);
        }

        // GET: Meals/Create
        public IActionResult Create()
        {
            ViewBag.Students = _context.Students.ToList();
            ViewBag.IssuedBy = User.Identity?.Name ?? "System";
            return View();
        }

        // POST: Meals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int studentId, int validDays, string issuedBy)
        {
            if (!_context.Students.Any(s => s.Id == studentId))
                return NotFound("Student not found");

            var mealCard = new MealCard
            {
                StudentId = studentId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(validDays),
                IsActive = true,
                IssuedBy = issuedBy,
                Remarks = $"Valid for {validDays} days"
            };

            _context.MealCards.Add(mealCard);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Meals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var mealCard = await _context.MealCards
                .Include(m => m.Student)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mealCard == null) return NotFound();

            ViewBag.Students = _context.Students.ToList();
            return View(mealCard);
        }

        // POST: Meals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MealCard mealCard)
        {
            if (id != mealCard.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mealCard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MealCardExists(mealCard.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Students = _context.Students.ToList();
            return View(mealCard);
        }

        // POST: Meals/Deactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var mealCard = await _context.MealCards.FindAsync(id);
            if (mealCard == null)
                return NotFound();

            mealCard.IsActive = false;
            mealCard.Remarks += " (Deactivated)";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool MealCardExists(int id) => _context.MealCards.Any(e => e.Id == id);
    }
}