namespace AsumbiCampusSystem.Models
{
    public class LearningArea
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string TopicName { get; set; } = "";
        public string Description { get; set; } = "";
    }
}