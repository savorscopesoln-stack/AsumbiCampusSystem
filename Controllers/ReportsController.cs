using Microsoft.AspNetCore.Mvc;
using AsumbiCampusSystem.Helpers;

namespace AsumbiCampusSystem.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Weekly()
        {
            if (!RoleHelper.HasAnyRole(HttpContext, "Master Admin", "Deputy Admin", "Teacher On Duty"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View();
        }
    }
}