using Microsoft.EntityFrameworkCore;

namespace ProblemAssignment2Rafia.Entitties
{
    public class CourseStudentDbContext:DbContext
    {
        public CourseStudentDbContext(DbContextOptions<CourseStudentDbContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            // Map enum values --> strings in DB based on enum name:
            modelBuilder.Entity<Student>()
                .Property(inv => inv.Status)
                .HasConversion<string>()
                .HasMaxLength(64);

            // Explicitly configure the one-to-many relationship between Course and Students
            modelBuilder
                .Entity<Course>()
                .HasMany(c => c.Students)
                .WithOne(s => s.Course)
                .HasForeignKey(s => s.CourseId);

            // Seed data
             modelBuilder.Entity<Course>().HasData(
                  new Course()
                  {
                        CourseId = 1, 
                        Name = "Programming Concept I",
                        Instructor = "Jane Doe",
                        StartDate = new DateTime(2024, 1, 18),
                        RoomNumber = "B101"
                  },
                  new Course()
                  {
                        CourseId = 2,
                        Name = "Advanced Databases",
                        Instructor = "John Smith",
                        StartDate = new DateTime(2024, 2, 12),
                        RoomNumber = "B105"
                  });
             modelBuilder.Entity<Student>().HasData(
                 new Student()
                 {
                     StudentId = 1,
                     Name = "Alice Johnson",
                     Email = "alice.johnson@example.com",
                     Status = EnrollmentStatus.ConfirmationMessageSent,
                     CourseId = 1 
                 },
                 new Student()
                 {
                      StudentId = 2,
                      Name = "Bob Brown",
                      Email = "bob.brown@example.com",
                      Status = EnrollmentStatus.EnrollmentConfirmed,
                      CourseId = 2 
                 });
        }

    }
}
