using Microsoft.AspNetCore.Mvc;
using AsumbiCampusSystem.Data;
using AsumbiCampusSystem.Models;
using System;
using System.Linq;

namespace AsumbiCampusSystem.Controllers
{
    public class GateStaffController : Controller
    {
        private readonly AppDbContextNew _context;

        public GateStaffController(AppDbContextNew context)
        {
            _context = context;
        }

        // GET: Scan page
        public IActionResult Scan()
        {
            return View();
        }

        // POST: Scan RFID
        [HttpPost]
        public IActionResult Scan(string rfidUID)
        {
            var student = _context.Students.FirstOrDefault(s => s.RFID_UID == rfidUID);
            if (student == null)
            {
                TempData["Message"] = "Student not found. Exit Denied.";
                return RedirectToAction("Scan");
            }

            // Find today's leave record (Approved or Active)
            var leave = _context.LeaveRecords
                .Where(l => l.StudentName == student.FullName && 
                            (l.Status == "Approved" || l.Status == "Active") &&
                            l.TimeOut.Date == DateTime.Today)
                .OrderByDescending(l => l.TimeOut)
                .FirstOrDefault();

            if (leave != null)
            {
                if (leave.Status == "Approved")
                {
                    // First scan → student leaving
                    leave.Status = "Active";
                    leave.GateStatus = "Left";
                    _context.LeaveRecords.Update(leave);
                    _context.SaveChanges();
                    TempData["Message"] = $"{student.FullName} exit allowed. Leave Active.";
                }
                else if (leave.Status == "Active" && leave.GateStatus == "Left")
                {
                    // Second scan → student returning
                    leave.Status = "Completed";
                    leave.GateStatus = "Returned";
                    leave.ActualReturn = DateTime.Now;
                    _context.LeaveRecords.Update(leave);
                    _context.SaveChanges();
                    TempData["Message"] = $"{student.FullName} returned. Leave Completed.";
                }
            }
            else
            {
                // No approved leave → deny exit
                var deniedLeave = new LeaveRecord
                {
                    StudentName = student.FullName,
                    AdmissionNumber = student.AdmissionNumber,
                    ClassName = student.ClassName,
                    Reason = "No approved leave",
                    TimeOut = DateTime.Now,
                    ExpectedReturn = DateTime.Now,
                    Status = "Denied",
                    GateStatus = "Rejected"
                };
                _context.LeaveRecords.Add(deniedLeave);
                _context.SaveChanges();
                TempData["Message"] = $"{student.FullName} exit denied. No approved leave.";
            }

            return RedirectToAction("Scan");
        }

        // Dashboard view for gate staff
        public IActionResult Dashboard()
        {
            var leaves = _context.LeaveRecords.OrderByDescending(l => l.TimeOut).ToList();
            return View(leaves);
        }
    }
}