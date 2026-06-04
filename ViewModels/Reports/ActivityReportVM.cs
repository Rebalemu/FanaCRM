namespace FanaCRM.ViewModels.Reports
{
    public class ActivityReportVM
    {
        public int TotalActivities { get; set; }

        public int CompletedActivities { get; set; }

        public int PendingActivities { get; set; }

        public int OverdueActivities { get; set; }

        public List<ActivityPerformanceRowVM> UserPerformance { get; set; } = new();

        public List<ChartItemVM> ActivitiesByType { get; set; } = new();

        public List<ChartItemVM> ActivitiesByStatus { get; set; } = new();
    }
}