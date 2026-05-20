namespace FanaCRM.ViewModels
{
    public class ActivityWidgetVM
    {
        public int Id { get; set; }

        public string Subject { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsOverdue { get; set; }
    }
}