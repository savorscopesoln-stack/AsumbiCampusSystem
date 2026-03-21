using Microsoft.AspNetCore.Mvc;
using AsumbiCampusSystem.Data;
using AsumbiCampusSystem.Helpers;

namespace AsumbiCampusSystem.Controllers
{
    public class NotificationController : Controller
    {
        private readonly AppDbContextNew _context;
        public NotificationController(AppDbContextNew context) { _context = context; }

        [HttpPost]
        public IActionResult MarkAsRead(int id)
        {
            NotificationHelper.MarkAsRead(_context, id);
            return Ok();
        }
    }
}