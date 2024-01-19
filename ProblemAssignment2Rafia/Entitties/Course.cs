using System.ComponentModel.DataAnnotations;

namespace ProblemAssignment2Rafia.Entitties
{
    public class Course
    {
       //pk
        public int CourseId { get; set; }

        [Required(ErrorMessage = "A Name for the course is required.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "An Instructor for the course is required.")]
        public string? Instructor { get; set; }

        [Required(ErrorMessage = "A start date for the course is required.")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "A room number for the course is required.")]
        [RegularExpression(@"^\d[A-Z]\d{2}$",
            ErrorMessage = "The Room number must be in correct format(e.g. 3G15, 1C07).")]
        public string? RoomNumber { get; set; }

        // Navigation property for the related Student entities
        public ICollection<Student>? Students { get; set; }
    }
}
