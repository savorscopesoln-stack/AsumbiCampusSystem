using System;

namespace AsumbiCampusSystem.Models
{
    public class RFIDLog
    {
        public int Id { get; set; }
        public string RFID_UID { get; set; } = "";
        public string StudentOrStaffName { get; set; } = "";
        public string Role { get; set; } = ""; // "Student" or "Staff"
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}