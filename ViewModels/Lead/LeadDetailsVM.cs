namespace FanaCRM.ViewModels
{
    public class LeadDetailsVM
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Company { get; set; }

        public string Status { get; set; }

        public string Source { get; set; }

        public string AssignedTo { get; set; }

        public DateTime CreatedDate { get; set; }

        // =====================================
        // TIMELINE
        // =====================================

        public List<TimelineEventVM> Timeline { get; set; } = new();
         public List<ActivityWidgetVM> UpcomingActivities { get; set; } = new();
        
    }
}