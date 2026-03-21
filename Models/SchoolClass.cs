namespace AsumbiCampusSystem.Models
{
    public class SchoolClass
    {
        public int Id { get; set; }
        public string Name { get; set; } = ""; // e.g., "Form 1A"
        public string TeacherInCharge { get; set; } = "";
        public string Status { get; set; } = "Active";
    }
}