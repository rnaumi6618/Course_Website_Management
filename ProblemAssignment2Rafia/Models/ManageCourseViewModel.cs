using ProblemAssignment2Rafia.Entitties;
using System.ComponentModel.DataAnnotations;

namespace ProblemAssignment2Rafia.Models
{
    public class ManageCourseViewModel
    {
        public Course? ActiveCourse { get; set; }

        [Required(ErrorMessage = "Name & email is required.")]
        public Student? NewStudent { get; set; }
        
        public int ConfirmationMessageNotSentCount { get; set; }

        public int ConfirmationMessageSentCount { get; set; }

        public int EnrollmentConfirmedCount { get; set; }

        public int EnrollmentDeclinedCount { get; set; }

    }
}
