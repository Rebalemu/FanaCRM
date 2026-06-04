namespace FanaCRM.ViewModels
{
    public class ActivityDetailsVM
    {
        public int Id { get; set; }

        public string TypeName { get; set; }

        public string Subject { get; set; }

        public string? Description { get; set; }

        public string? CompanyName { get; set; }

        public string? ContactName { get; set; }

        public string? LeadName { get; set; }

        public string? OpportunityName { get; set; }

        public string AssignedTo { get; set; }

        public DateTime? DueDate { get; set; }

        public string Status { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}