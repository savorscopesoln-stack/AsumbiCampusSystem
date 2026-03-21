namespace AsumbiCampusSystem.Models
{
    public class Cat
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; } = "";
        public DateTime DateAssigned { get; set; } = DateTime.Now;
        public DateTime? DateDue { get; set; }
    }
}