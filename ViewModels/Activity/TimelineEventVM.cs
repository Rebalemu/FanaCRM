namespace FanaCRM.ViewModels
{
    public class TimelineEventVM
    {
        public string Title { get; set; }

        public string? Description { get; set; }

        public string EventType { get; set; }

        public string UserName { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}