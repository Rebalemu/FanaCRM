
namespace FanaCRM.ViewModels
{
    public class ActivityIndexVM
    {
        public int Id { get; set; }

        public string TypeName { get; set; }

        public string Subject { get; set; }

        public string CompanyName { get; set; }

        public string ContactName { get; set; }

        public string AssignedTo { get; set; }

        public DateTime? DueDate { get; set; }

        public string Status { get; set; }
    }
}
