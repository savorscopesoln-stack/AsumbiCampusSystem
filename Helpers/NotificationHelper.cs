using AsumbiCampusSystem.Data;
using AsumbiCampusSystem.Models;
using System.Linq;

namespace AsumbiCampusSystem.Helpers
{
    public static class NotificationHelper
    {
        public static void Send(AppDbContextNew context, int userId, string title, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                IsRead = false
            };
            context.Notifications.Add(notification);
            context.SaveChanges();
        }

        public static IQueryable<Notification> GetUnread(AppDbContextNew context, int userId)
        {
            return context.Notifications.Where(n => n.UserId == userId && !n.IsRead)
                                        .OrderByDescending(n => n.CreatedAt);
        }

        public static void MarkAsRead(AppDbContextNew context, int id)
        {
            var n = context.Notifications.Find(id);
            if (n != null)
            {
                n.IsRead = true;
                context.SaveChanges();
            }
        }
    }
}