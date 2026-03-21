// Models/LeaveRecord.cs
using System;

namespace AsumbiCampusSystem.Models
{
    public class LeaveRecord
    {
        public int Id { get; set; }
        public string AdmissionNumber { get; set; } = "";
        public string StudentName { get; set; } = "";  // rename from "Student"
        public string ClassName { get; set; } = "";
        public string Reason { get; set; } = "";
        public DateTime TimeOut { get; set; }          // the leave start time
        public DateTime ExpectedReturn { get; set; }  // expected return
        public DateTime? ActualReturn { get; set; }
        public string Status { get; set; } = "Pending";
        public string LeaveType { get; set; } = "Normal";
        public string ApprovedBy { get; set; } = "";
        public string ApprovedByRole { get; set; } = "";

        // ✅ For gate tracking
        public string GateStatus { get; set; } = ""; // "Left" / "Rejected" / ""

        // ✅ Optional: add DateRequested if your view wants it
        public DateTime DateRequested { get; set; } = DateTime.Now;
    }
}