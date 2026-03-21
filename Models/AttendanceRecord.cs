using System;

namespace AsumbiCampusSystem.Models
{
    public class AttendanceRecord
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = "";
        public string AdmissionNumber { get; set; } = "";
        public string ClassName { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Present"; // "Absent" or "Present"
        public string RecordedBy { get; set; } = "";
    }
}