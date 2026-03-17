namespace AsumbiCampusSystem.Models
{
    public class LeaveRecord
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = "";
        public string AdmissionNumber { get; set; } = "";
        public string Reason { get; set; } = "";
        public string TimeOut { get; set; } = "";
        public string ExpectedReturn { get; set; } = "";
        public string ActualReturn { get; set; } = "";
        public string Status { get; set; } = "Pending";
        public string ApprovedBy { get; set; } = "";
    }
}