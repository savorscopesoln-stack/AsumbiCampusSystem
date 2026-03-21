using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using ClosedXML.Excel;
using AsumbiCampusSystem.Data;
using AsumbiCampusSystem.Models;
using AsumbiCampusSystem.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;

namespace AsumbiCampusSystem.Controllers
{
    public class StudentsController : Controller
    {
        private readonly AppDbContextNew _context;

        public StudentsController(AppDbContextNew context)
        {
            _context = context;
        }

        private string GetUserRole() => HttpContext.Session.GetString("Role") ?? "";

        // ✅ Allow Dean without removing anything
        private bool Allowed() =>
            RoleHelper.HasAnyRole(HttpContext, "Master Admin", "Deputy Admin", "Class Teacher", "Dean of Students");

        // GET: Students
        public async Task<IActionResult> Index()
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            var students = await _context.Students.ToListAsync();
            return View(students);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (!Allowed()) return RedirectToAction("AccessDenied", "Account");

            if (id != student.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Student record updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }

        // ==========================
        // PDF EXPORT
        // ==========================
        public async Task<IActionResult> ExportPdf(string type, string date)
        {
            DateTime? parsedDate = null;
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var dt))
                parsedDate = dt.Date;

            switch (type?.ToLower())
            {
                case "students":
                    var students = await _context.Students.ToListAsync();
                    return new ViewAsPdf("PdfTemplates/Students", students) { FileName = "StudentsReport.pdf" };

                case "staff":
                    var staff = await _context.StaffMembers.ToListAsync();
                    return new ViewAsPdf("PdfTemplates/Staff", staff) { FileName = "StaffReport.pdf" };

                case "attendance":
                    var attendance = await _context.AttendanceRecords
                        .Where(a => !parsedDate.HasValue || a.Date.Date == parsedDate.Value)
                        .ToListAsync();
                    return new ViewAsPdf("PdfTemplates/AttendanceRecords", attendance) { FileName = "AttendanceReport.pdf" };

                case "activeleave":
                    var activeLeaves = await _context.LeaveRecords
                        .Where(l => l.Status == "Approved" &&
                                    (!parsedDate.HasValue || (l.TimeOut.Date <= parsedDate.Value && l.ExpectedReturn.Date >= parsedDate.Value)))
                        .ToListAsync();
                    return new ViewAsPdf("PdfTemplates/ActiveLeaves", activeLeaves) { FileName = "ActiveLeaves.pdf" };

                case "overdueleave":
                    var overdueLeaves = await _context.LeaveRecords
                        .Where(l => l.Status == "Overdue" &&
                                    (!parsedDate.HasValue || l.ExpectedReturn.Date <= parsedDate.Value))
                        .ToListAsync();
                    return new ViewAsPdf("PdfTemplates/OverdueLeaves", overdueLeaves) { FileName = "OverdueLeaves.pdf" };

                case "meals":
                    var meals = await _context.MealRecords
                        .Where(m => !parsedDate.HasValue || m.Date.Date == parsedDate.Value) // Fixed nullable
                        .ToListAsync();
                    return new ViewAsPdf("PdfTemplates/Meals", meals) { FileName = "MealsReport.pdf" };

                default:
                    return RedirectToAction("Index");
            }
        }

        // ==========================
        // EXCEL EXPORT
        // ==========================
        public async Task<IActionResult> ExportExcel(string type, string date)
        {
            DateTime? parsedDate = null;
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var dt))
                parsedDate = dt.Date;

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Report");
            int row = 1;

            switch (type?.ToLower())
            {
                case "students":
                    sheet.Cell(row, 1).Value = "Full Name";
                    sheet.Cell(row, 2).Value = "Admission Number";
                    sheet.Cell(row, 3).Value = "Class";
                    var students = await _context.Students.ToListAsync();
                    foreach (var s in students)
                    {
                        row++;
                        sheet.Cell(row, 1).Value = s.FullName;
                        sheet.Cell(row, 2).Value = s.AdmissionNumber;
                        sheet.Cell(row, 3).Value = s.ClassName;
                    }
                    break;

                case "staff":
                    sheet.Cell(row, 1).Value = "Full Name";
                    sheet.Cell(row, 2).Value = "Staff Number";
                    sheet.Cell(row, 3).Value = "Department";
                    sheet.Cell(row, 4).Value = "Phone Number";
                    var staff = await _context.StaffMembers.ToListAsync();
                    foreach (var s in staff)
                    {
                        row++;
                        sheet.Cell(row, 1).Value = s.FullName;
                        sheet.Cell(row, 2).Value = s.StaffNumber;
                        sheet.Cell(row, 3).Value = s.Department;
                        sheet.Cell(row, 4).Value = s.PhoneNumber;
                    }
                    break;

                case "attendance":
                    sheet.Cell(row, 1).Value = "Student Name";
                    sheet.Cell(row, 2).Value = "Admission Number";
                    sheet.Cell(row, 3).Value = "Class";
                    sheet.Cell(row, 4).Value = "Date";
                    sheet.Cell(row, 5).Value = "Status";
                    var attendance = await _context.AttendanceRecords
                        .Where(a => !parsedDate.HasValue || a.Date.Date == parsedDate.Value)
                        .ToListAsync();
                    foreach (var a in attendance)
                    {
                        row++;
                        sheet.Cell(row, 1).Value = a.StudentName;
                        sheet.Cell(row, 2).Value = a.AdmissionNumber;
                        sheet.Cell(row, 3).Value = a.ClassName;
                        sheet.Cell(row, 4).Value = a.Date.ToString("yyyy-MM-dd");
                        sheet.Cell(row, 5).Value = a.Status;
                    }
                    break;

                case "activeleave":
                    sheet.Cell(row, 1).Value = "Student Name";
                    sheet.Cell(row, 2).Value = "Class";
                    sheet.Cell(row, 3).Value = "Reason";
                    sheet.Cell(row, 4).Value = "Time Out";
                    sheet.Cell(row, 5).Value = "Expected Return";
                    sheet.Cell(row, 6).Value = "Actual Return";
                    sheet.Cell(row, 7).Value = "Status";
                    var activeLeaves = await _context.LeaveRecords
                        .Where(l => l.Status == "Approved" &&
                                    (!parsedDate.HasValue || (l.TimeOut.Date <= parsedDate.Value && l.ExpectedReturn.Date >= parsedDate.Value)))
                        .ToListAsync();
                    foreach (var l in activeLeaves)
                    {
                        row++;
                        sheet.Cell(row, 1).Value = l.StudentName;
                        sheet.Cell(row, 2).Value = l.ClassName;
                        sheet.Cell(row, 3).Value = l.Reason;
                        sheet.Cell(row, 4).Value = l.TimeOut.ToString("yyyy-MM-dd");
                        sheet.Cell(row, 5).Value = l.ExpectedReturn.ToString("yyyy-MM-dd");
                        sheet.Cell(row, 6).Value = l.ActualReturn?.ToString("yyyy-MM-dd") ?? "";
                        sheet.Cell(row, 7).Value = l.Status;
                    }
                    break;

                case "overdueleave":
                    sheet.Cell(row, 1).Value = "Student Name";
                    sheet.Cell(row, 2).Value = "Class";
                    sheet.Cell(row, 3).Value = "Reason";
                    sheet.Cell(row, 4).Value = "Time Out";
                    sheet.Cell(row, 5).Value = "Expected Return";
                    sheet.Cell(row, 6).Value = "Actual Return";
                    sheet.Cell(row, 7).Value = "Status";
                    var overdueLeaves = await _context.LeaveRecords
                        .Where(l => l.Status == "Overdue" &&
                                    (!parsedDate.HasValue || l.ExpectedReturn.Date <= parsedDate.Value))
                        .ToListAsync();
                    foreach (var l in overdueLeaves)
                    {
                        row++;
                        sheet.Cell(row, 1).Value = l.StudentName;
                        sheet.Cell(row, 2).Value = l.ClassName;
                        sheet.Cell(row, 3).Value = l.Reason;
                        sheet.Cell(row, 4).Value = l.TimeOut.ToString("yyyy-MM-dd");
                        sheet.Cell(row, 5).Value = l.ExpectedReturn.ToString("yyyy-MM-dd");
                        sheet.Cell(row, 6).Value = l.ActualReturn?.ToString("yyyy-MM-dd") ?? "";
                        sheet.Cell(row, 7).Value = l.Status;
                    }
                    break;

                case "meals":
                    sheet.Cell(row, 1).Value = "Student Name";
                    sheet.Cell(row, 2).Value = "Admission Number";
                    sheet.Cell(row, 3).Value = "Meal Type";
                    sheet.Cell(row, 4).Value = "Date";
                    sheet.Cell(row, 5).Value = "Status";
                    var meals = await _context.MealRecords
                        .Where(m => !parsedDate.HasValue || m.Date.Date == parsedDate.Value) // Fixed nullable
                        .ToListAsync();
                    foreach (var m in meals)
                    {
                        row++;
                        sheet.Cell(row, 1).Value = m.StudentName;
                        sheet.Cell(row, 2).Value = m.AdmissionNumber;
                        sheet.Cell(row, 3).Value = m.MealType;
                        sheet.Cell(row, 4).Value = m.Date.ToString("yyyy-MM-dd");
                        sheet.Cell(row, 5).Value = m.Status;
                    }
                    break;

                default:
                    return RedirectToAction("Index");
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(), 
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                        "Report.xlsx");
        }

        public IActionResult AccessDenied() => View();
    }
}