namespace AsumbiCampusSystem.Models
{
    public class StaffMember
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string StaffNumber { get; set; } = "";
        public string Department { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string RFID_UID { get; set; } = "";
        public string Status { get; set; } = "Active";
    }
}