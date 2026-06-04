namespace FanaCRM.ViewModels
{
    public class ActivityDashboardVM
    {
        // COUNTERS
        public int TotalActivities { get; set; }

        public int PendingActivities { get; set; }

        public int CompletedToday { get; set; }

        public int OverdueActivities { get; set; }

        // LISTS
        public List<ActivityIndexVM> MyActivities { get; set; } = new();

        public List<ActivityIndexVM> UpcomingActivities { get; set; } = new();

        public List<ActivityIndexVM> OverdueList { get; set; } = new();

        public List<ActivityIndexVM> CompletedTodayList { get; set; } = new();
    }
}