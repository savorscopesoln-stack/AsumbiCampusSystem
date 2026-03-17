namespace AsumbiCampusSystem.Models
{
    public class MealRecord
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = "";
        public string AdmissionNumber { get; set; } = "";
        public string MealType { get; set; } = "";
        public string Date { get; set; } = "";
        public string Status { get; set; } = "Served";
    }
}