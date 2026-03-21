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
    public class DashboardController : Controller
    {
        private readonly AppDbContextNew _context;

        public DashboardController(AppDbContextNew context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string role = HttpContext.Session.GetString("Role") ?? "";

            // Only allow Master Admin, Deputy Admin, TOD, Dean
            if (!RoleHelper.HasAnyRole(HttpContext, "Master Admin", "Deputy Admin", "Teacher On Duty", "Dean of Students"))
                return RedirectToAction("AccessDenied", "Account");

            // Fetch all data
            var students = await _context.Students.ToListAsync();
            var staff = await _context.StaffMembers.ToListAsync();
            var leaves = await _context.LeaveRecords.ToListAsync();
            var attendance = await _context.AttendanceRecords.ToListAsync();
            var meals = await _context.MealRecords.ToListAsync();

            // ==========================
            // STATS
            // ==========================
            ViewBag.TotalStudents = students.Count;

            // Staff count only for admins
            ViewBag.TotalStaff = (role == "Master Admin" || role == "Deputy Admin") ? staff.Count : 0;

            // Active leaves visible based on role
            ViewBag.ActiveLeaves = leaves.Count(l =>
                l.Status == "Approved" &&
                ((role == "Dean of Students" && (l.ExpectedReturn - l.TimeOut).TotalDays <= 14) || role != "Dean of Students")
            );

            ViewBag.OverdueReturns = leaves.Count(l => l.Status == "Overdue");

            // Meals today: all roles can see
            ViewBag.MealsServedToday = meals.Count(m => m.Date.Date == DateTime.Today);

            // Attendance today: only admin sees full attendance
            ViewBag.AttendanceToday = (role == "Master Admin" || role == "Deputy Admin") ? attendance.Count(a => a.Date.Date == DateTime.Today) : 0;

            // ==========================
            // Charts (Admin only)
            // ==========================
            if (role == "Master Admin" || role == "Deputy Admin")
            {
                ViewBag.AttendanceChart = new int[] { 900, 1000, 950, 1100, 1050, 1150 };
                ViewBag.LeaveChart = new int[] { 5, 8, 4, 6, 3, 7 };
            }

            // ==========================
            // Pending Leave Approvals
            // ==========================
            ViewBag.PendingLeaves = leaves
                .Where(l => l.Status == "Pending" &&
                           ((role == "Deputy Admin" && (l.ExpectedReturn - l.TimeOut).TotalDays > 14) ||
                            (role == "Dean of Students" && (l.ExpectedReturn - l.TimeOut).TotalDays <= 14) ||
                            role == "Master Admin" ||
                            (role == "Teacher On Duty" && (l.ExpectedReturn - l.TimeOut).TotalDays <= 1)))
                .ToList();

            // Notifications for pending leaves
            var unreadNotifications = new List<Notification>();
            foreach (var leave in ViewBag.PendingLeaves)
            {
                unreadNotifications.Add(new Notification
                {
                    Id = leave.Id,
                    Title = "Leave Approval Required",
                    Message = $"{leave.StudentName} ({leave.ClassName}) has requested leave from {leave.TimeOut:dd/MM/yyyy} to {leave.ExpectedReturn:dd/MM/yyyy}.",
                    CreatedAt = leave.TimeOut
                });
            }
            ViewBag.UnreadNotifications = unreadNotifications;
            ViewBag.UnreadCount = unreadNotifications.Count;

            return View();
        }

        // ==========================
        // TOD Approve Leave
        // ==========================
        [HttpPost]
        public async Task<IActionResult> ApproveLeave(int id)
        {
            var leave = await _context.LeaveRecords.FindAsync(id);
            if (leave == null) return Json(new { success = false, message = "Leave not found" });

            string role = HttpContext.Session.GetString("Role") ?? "";

            // Only allow TOD to approve 1-day leaves
            if (role == "Teacher On Duty" && (leave.ExpectedReturn - leave.TimeOut).TotalDays > 1)
                return Json(new { success = false, message = "TOD can only approve 1-day leaves." });

            leave.Status = "Approved";
            leave.ApprovedBy = HttpContext.Session.GetString("UserName") ?? "";
            leave.ApprovedByRole = role;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // ==========================
        // Mark Leave as Returned
        // ==========================
        [HttpPost]
        public async Task<IActionResult> MarkLeaveReturned(int id)
        {
            var leave = await _context.LeaveRecords.FindAsync(id);
            if (leave == null) return Json(new { success = false, message = "Leave not found" });

            leave.Status = "Completed";
            leave.ActualReturn = DateTime.Now;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
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
                        .Where(a => parsedDate == null || a.Date.Date == parsedDate.Value)
                        .ToListAsync();
                    return new ViewAsPdf("PdfTemplates/AttendanceRecords", attendance) { FileName = "AttendanceReport.pdf" };

                case "activeleave":
                    var activeLeaves = await _context.LeaveRecords
                        .Where(l => l.Status == "Approved" &&
                                    (parsedDate == null || (l.TimeOut.Date <= parsedDate.Value && l.ExpectedReturn.Date >= parsedDate.Value)))
                        .ToListAsync();
                    return new ViewAsPdf("PdfTemplates/ActiveLeaves", activeLeaves) { FileName = "ActiveLeaves.pdf" };

                case "overdueleave":
                    var overdueLeaves = await _context.LeaveRecords
                        .Where(l => l.Status == "Overdue" &&
                                    (parsedDate == null || l.ExpectedReturn.Date <= parsedDate.Value))
                        .ToListAsync();
                    return new ViewAsPdf("PdfTemplates/OverdueLeaves", overdueLeaves) { FileName = "OverdueLeaves.pdf" };

                case "meals":
                    var meals = await _context.MealRecords
                        .Where(m => parsedDate == null || m.Date.Date == parsedDate.Value)
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
                        .Where(a => parsedDate == null || a.Date.Date == parsedDate.Value)
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
                case "overdueleave":
                    var leaves = await _context.LeaveRecords
                        .Where(l => type.ToLower() == "activeleave" ? 
                                    l.Status == "Approved" : 
                                    l.Status == "Overdue")
                        .Where(l => parsedDate == null || 
                                    (type.ToLower() == "activeleave" ? 
                                     (l.TimeOut.Date <= parsedDate.Value && l.ExpectedReturn.Date >= parsedDate.Value) :
                                     l.ExpectedReturn.Date <= parsedDate.Value))
                        .ToListAsync();
                    sheet.Cell(row, 1).Value = "Student Name";
                    sheet.Cell(row, 2).Value = "Class";
                    sheet.Cell(row, 3).Value = "Reason";
                    sheet.Cell(row, 4).Value = "Time Out";
                    sheet.Cell(row, 5).Value = "Expected Return";
                    sheet.Cell(row, 6).Value = "Actual Return";
                    sheet.Cell(row, 7).Value = "Status";
                    foreach (var l in leaves)
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
                        .Where(m => parsedDate == null || m.Date.Date == parsedDate.Value)
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