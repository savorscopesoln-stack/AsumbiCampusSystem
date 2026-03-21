using System;

namespace AsumbiCampusSystem.Models
{
    public class Result
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int LearningAreaId { get; set; }
        public string CATName { get; set; } = string.Empty; // required
        public double Score { get; set; }
        public DateTime? DateRecorded { get; set; }
    }
}