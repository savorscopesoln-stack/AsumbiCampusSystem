using Microsoft.AspNetCore.Mvc;
using AsumbiCampusSystem.Helpers;

namespace AsumbiCampusSystem.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            if (!RoleHelper.HasAnyRole(HttpContext, "Master Admin", "Deputy Admin"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            ViewBag.TotalStudents = 1200;
            ViewBag.TotalStaff = 85;
            ViewBag.ActiveLeaves = 14;
            ViewBag.OverdueReturns = 3;
            ViewBag.MealsServedToday = 760;
            ViewBag.AttendanceToday = 1098;

            return View();
        }
    }
}