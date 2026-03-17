namespace AsumbiCampusSystem.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string AdmissionNumber { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string ParentPhone { get; set; } = "";
        public string RFID_UID { get; set; } = "";
        public string Status { get; set; } = "Active";
    }
}