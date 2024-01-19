using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProblemAssignment2Rafia.Entitties
{
    public class Student
    {
        //PK
        public int StudentId { get; set; }

        [Required(ErrorMessage = "A name for the student is required.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "An email address for the student is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string? Email { get; set; }

        [Required]
        public EnrollmentStatus? Status { get; set; } = EnrollmentStatus.ConfirmationMessageNotSent;

        // Foreign key for the Course this student is associated with
        [ForeignKey("Course")]
        public int CourseId { get; set; }

        // Navigation property for the related Course entity
        public Course? Course { get; set; }
    }
    public enum EnrollmentStatus
    {
         ConfirmationMessageNotSent=0,
         ConfirmationMessageSent,
         EnrollmentConfirmed,
         EnrollmentDeclined
    }
}
