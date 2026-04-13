using System.ComponentModel.DataAnnotations;

namespace FanaCRM.ViewModels
{
    public class LeadIndexVM
    {
        public int Id { get; set; }

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Company { get; set; }

        public string? Source { get; set; }
        public string? Status { get; set; }

        [Display(Name = "Assigned To")]
        public string? AssignedTo { get; set; }

        [Display(Name = "Created On")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime CreatedDate { get; set; }
    }
}