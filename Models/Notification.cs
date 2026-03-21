using System;

namespace AsumbiCampusSystem.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }             // Must match your users table type
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}