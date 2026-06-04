
using System.ComponentModel.DataAnnotations;

namespace FanaCRM.ViewModels
{
    public class ContactIndexVM
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? Position { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}