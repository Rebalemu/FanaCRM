using System.ComponentModel.DataAnnotations;
using FanaCRM.Models;

namespace FanaCRM.ViewModels
{
    public class CompanyIndexVM
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Industry { get; set; }
        public string Phone { get; set; }
        public int ContactCount { get; set; }
    }
}