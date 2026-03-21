using System;
using System.Collections.Generic;
using AsumbiCampusSystem.Models;

namespace AsumbiCampusSystem.Data
{
    public static class InMemoryData
    {
        // ==========================
        // STUDENTS
        // ==========================
        public static List<Student> Students = new List<Student>
        {
            new Student { Id = 1, FullName = "John Otieno", AdmissionNumber = "ATC001", ClassName = "Form 1" },
            new Student { Id = 2, FullName = "Mary Achieng", AdmissionNumber = "ATC002", ClassName = "Form 2" }
        };

        // ==========================
        // COURSES
        // ==========================
        public static List<Course> Courses = new List<Course>
        {
            new Course { Id = 1, CourseName = "Mathematics" },
            new Course { Id = 2, CourseName = "English" }
        };

        // ==========================
        // ENROLLMENTS
        // ==========================
        public static List<Enrollment> Enrollments = new List<Enrollment>
        {
            new Enrollment { Id = 1, StudentId = 1, CourseId = 1, EnrolledOn = DateTime.Now }
        };

        // ==========================
        // LEARNING AREAS
        // ==========================
        public static List<LearningArea> LearningAreas = new List<LearningArea>
        {
            new LearningArea { Id = 1, CourseId = 1, TopicName = "Algebra", Description = "Introduction to Algebra" },
            new LearningArea { Id = 2, CourseId = 1, TopicName = "Geometry", Description = "Basic Shapes and Angles" }
        };

        // ==========================
        // RESULTS (FIXED ✅)
        // ==========================
        public static List<Result> Results = new List<Result>
        {
            new Result { Id = 1, StudentId = 1, CourseId = 1, LearningAreaId = 1, CATName = "CAT 1", Score = 75 },
            new Result { Id = 2, StudentId = 1, CourseId = 1, LearningAreaId = 2, CATName = "CAT 2", Score = 80 }
        };
    }
}