namespace AsumbiCampusSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = ""; // Plain text for demo
        public string Role { get; set; } = "";     // Master Admin, Deputy Admin, TOD, Dean, etc.
        public int? StudentId { get; set; }        
        public Student? Student { get; set; }      
    }
}