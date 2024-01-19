using ProblemAssignment2Rafia.Entitties;
using System.ComponentModel.DataAnnotations;

namespace ProblemAssignment2Rafia.Models
{
    public class ConfirmationViewModel
    {
        [Required(ErrorMessage = "You must choose a response.")]
        public bool? Confirmation { get; set; } 
        public Course Course { get; set; }
        public Student Student { get; set; }
    }
}
