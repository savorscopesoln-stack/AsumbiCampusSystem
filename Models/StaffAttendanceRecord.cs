namespace AsumbiCampusSystem.Models
{
    public class StaffAttendanceRecord
    {
        public int Id { get; set; }
        public string StaffName { get; set; } = "";
        public string StaffNumber { get; set; } = "";
        public string Department { get; set; } = "";
        public string Date { get; set; } = "";
        public string Status { get; set; } = "Present";
        public string RecordedBy { get; set; } = "";
    }
}