namespace AsumbiCampusSystem.Models
{
    public class AttendanceRecord
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = "";
        public string AdmissionNumber { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string Date { get; set; } = "";
        public string Status { get; set; } = "Present";
        public string RecordedBy { get; set; } = "";
    }
}