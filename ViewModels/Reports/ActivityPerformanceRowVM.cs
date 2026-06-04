namespace FanaCRM.ViewModels.Reports
{
    public class ActivityPerformanceRowVM
    {
        public string? UserName { get; set; }

        public int Calls { get; set; }

        public int Meetings { get; set; }

        public int Emails { get; set; }

        public int Tasks { get; set; }

        public int Completed { get; set; }

        public int Pending { get; set; }

        public int Overdue { get; set; }
    }
}