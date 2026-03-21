namespace AsumbiCampusSystem.Models
{
    public class Mark
    {
        public int Id { get; set; }
        public int CatId { get; set; }
        public int StudentId { get; set; }
        public decimal Score { get; set; }
        public DateTime DateRecorded { get; set; } = DateTime.Now;
    }
}