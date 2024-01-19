using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProblemAssignment2Rafia.Entitties;
using ProblemAssignment2Rafia.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using static System.Net.WebRequestMethods;

namespace ProblemAssignment2Rafia.Controllers
{
    public class CourseController : AbstractBaseController
    {
        //because we add this here as constractor parameter
        //it means the DI container knows to create and pass one to us when this object are created
        public CourseController(CourseStudentDbContext courseStudentDbContext)
            {
            //store in our private data field to use in our action method
                _dbContext = courseStudentDbContext;
            }

            //use attribute routing to specify this as a get handler 
            //for the resource representing the collection of all courses
            [HttpGet("/my-courses")]
            public IActionResult GetAllCourses()
            {
                List<Course>courses = _dbContext.Courses
                                        .Include(c => c.Students)
                                        .OrderBy(m => m.Name)
                                        .ToList();

                //pass off this list to our view by name
                return View("Lists", courses);
            }

            //Get add form
            [HttpGet("/my-courses/add-form")]
            public IActionResult GetAddForm()
            {
            //return a form with an empty course view
            return View("Add", new Course());
            }

            //post method for add form
            [HttpPost("/my-courses")]
            public IActionResult AddNewCourse(Course course)
            {
                if (ModelState.IsValid)
                {
                    // fields are valid so add course to the DB & save:
                    _dbContext.Courses.Add(course);
                    _dbContext.SaveChanges();

                    TempData["Message"] = $"New Course {course.Name} added successfully!";

                // Redirect back to the all course view:
                return RedirectToAction("GetAllCourses", "Course");
                }
                else
                {
                    // there was a validn err so return the to the user
                    // along with any possible validn err msgs:
                    return View("Add", course);
                }
            }

            //edit a specific course by id
            //get method for edit
            [HttpGet("/my-courses/{id}/edit-form")]
            public IActionResult GetEditForm(int id)
            {
                //find the player for the id passes as parameter
                var course = _dbContext.Courses.Find(id);

                //and Return it to edit view:
                return View("Edit", course);
            }

            //post method for edit
            [HttpPost("/my-courses/edit-requests")]
            public IActionResult EditCourse(Course course)
            {
                if (ModelState.IsValid)
                {
                // fields are valid so update course to the DB & save:
                _dbContext.Courses.Update(course);
                    _dbContext.SaveChanges();

                    TempData["Message"] = $"Information of {course.Name} updated successfully!";

                // Redirect back to the all course view:
                return RedirectToAction("GetAllCourses", "Course");
                }
                else
                {
                    // there was a validn err so return  to the user
                    // along with any possible validn err msgs:
                    return View("Edit", course);
                }
            }

            //get method for details information of a specific course by id
            [HttpGet("/my-courses/{id}")]
            public IActionResult GetCourseById(int id)
            {
                //find the measurement for the id passes as parameter
                Course? course = _dbContext.Courses
                                     .Include(p => p.Students)
                                     .Where(p => p.CourseId == id)
                                     .FirstOrDefault();

              ManageCourseViewModel manageCourseViewModel = new ManageCourseViewModel()
              {
                ActiveCourse = course,
                NewStudent = new Student(),
                ConfirmationMessageNotSentCount = course.Students
                        .Where(s => s.Status == EnrollmentStatus.ConfirmationMessageNotSent).Count(),
                ConfirmationMessageSentCount = course.Students
                    .Where(s => s.Status == EnrollmentStatus.ConfirmationMessageSent).Count(),
                EnrollmentConfirmedCount = course.Students
                    .Where(s => s.Status == EnrollmentStatus.EnrollmentConfirmed).Count(),
                EnrollmentDeclinedCount = course.Students
                         .Where(s => s.Status == EnrollmentStatus.EnrollmentDeclined).Count()
              };

              //and Return it to details view:
              return View("Manage", manageCourseViewModel);
            }
        [HttpPost("/my-courses/{id}/students")]
        public IActionResult AddStudentToCourseById(int id, ManageCourseViewModel vm)
        {
            // Query for the course by the ID But need to include students
            Course? course = _dbContext.Courses
                .Include(c => c.Students)
                .Where(c => c.CourseId == id)
                .FirstOrDefault();
            if (ModelState.IsValid)
            {
                Student student = vm.NewStudent;

                if (course != null)
                {
                    course.Students.Add(student);
                }

                // Update the course with the new student
                _dbContext.Courses.Update(course);

                // Save changes
                _dbContext.SaveChanges();

                // Set a success message
                TempData["Message"] = $"New Student {student.Name} added successfully";

                // Redirect back to that same Manage page
                return RedirectToAction("GetCourseById", "Course", new { id = course.CourseId }); 
            }
            else
            {
                // there was a validn err so return  to the user
                // add error message and return to the same page
                return RedirectToAction("GetCourseById", "Course", new { id = course.CourseId });
            }
        }

        [HttpPost("/courses/{id}/send-enrollment-confirmation-requests")]
        async public Task<IActionResult> SendConfirmationRequestsByCourseId(int id)
        {
            Course? course = _dbContext.Courses
                                .Include(c => c.Students)
                                .FirstOrDefault(c => c.CourseId == id);
            
            string fromAddress = "bbao87521@gmail.com";
            string fromPassword = "hgfqrqopsvnqimzu";
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromAddress, fromPassword),
                EnableSsl = true,
            };

            foreach (Student student in course.Students)
            {
                if (student.Status == EnrollmentStatus.ConfirmationMessageNotSent)
                {
                    string url = $"https://localhost:7210/my-courses/{course.CourseId}/student/{student.StudentId}";
                    var mailMessage = new MailMessage()
                    {
                        From = new MailAddress(fromAddress),
                        Subject = $"Enrollment confirmation for {course.Name} required",
                        Body = $"<h1>Hello {student.Name},</h1>" +
                        $"<p>Your request to enroll in the course {course.Name} in room {course.RoomNumber} " +
                        $"starting {course.StartDate?.ToString("d")} with instructor {course.Instructor}." +
                        $"\n We are pleased to have you in the course " +
                        $"so if you could <a href=\"{url}\">confirm your enrollment</a> as soon as possible that would be appreciated!" +
                        $"\n Sincerely,\nThe Course Manager App</p>",
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(student.Email);
                    //TempData["LastActionMessage"] = $"Trying to send message {mailMessage.Body} with subject {mailMessage.Subject}";
                    try
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                    }
                    catch (SmtpException smtpEx)
                    {
                        TempData["Message"] = $"SMTP Error: {smtpEx.StatusCode}, Message: {smtpEx.Message}";
                    }
                    catch (Exception ex)
                    {
                        TempData["Message"] = $"Unknown Error: {ex.Message}";
                    }
                    student.Status = EnrollmentStatus.ConfirmationMessageSent;
                }
            }

            // Save the updated statuses of the students
              _dbContext.SaveChanges();

            // Set a success message
            TempData["Message"] = "Confirmation email sent successfully";

            // Redirect back to the Manage course page
            return RedirectToAction("GetCourseById", "Course", new { id = course.CourseId });
        }

        //get method for details information of a specific course by id
        [HttpGet("/my-courses/{course_id}/student/{student_id}")]
        public IActionResult GetConfirmationByStudent(int course_id, int student_id)
        {
            //find the measurement for the id passes as parameter
            Course? course = _dbContext.Courses
                 .Include(p => p.Students)
                .Where(p => p.CourseId == course_id)
                .FirstOrDefault();

            Student? student = _dbContext.Students
                .Where(p => p.StudentId == student_id)
                .FirstOrDefault();

            ConfirmationViewModel confirmationViewModel = new ConfirmationViewModel();
            confirmationViewModel.Student = student;
            confirmationViewModel.Course = course;

            //and Return it to details view:
            return View("Confirmation", confirmationViewModel);
        }
        [HttpPost("/my-courses/{courseId}/student/{studentId}/confirm-enrollment")]
        public IActionResult ConfirmEnrollment(int courseId, int studentId, string confirmationResponse)
        {
            // Find the student for the given IDs
            var student = _dbContext.Students
                          .FirstOrDefault
                          (s => s.StudentId == studentId && s.CourseId == courseId);

            // Update the student's enrollment status based on the confirmation response
            if (confirmationResponse == "yes")
            {
                student.Status = EnrollmentStatus.EnrollmentConfirmed;
            }
            else if (confirmationResponse == "no")
            {
                student.Status = EnrollmentStatus.EnrollmentDeclined;
            }
            else
            {
                // Handle the case where an invalid response was submitted
                TempData["Message"] = "Invalid response.";
                return BadRequest();
            }

            // Save the updated status
            _dbContext.Update(student);
            _dbContext.SaveChanges();

            // Set a success message
            TempData["Message"] = $"Enrollment status for {student.Name} updated successfully!";

            // Redirect back to the manage course page or a confirmation page
            return RedirectToAction("ThankYou", "Course");
        }
        [HttpGet("thank-you")]
        public IActionResult ThankYou()
        {
            // Set a success message
            TempData["Message"] = "Response submitted successfully";
            return View("Thanks");
        }
        private CourseStudentDbContext _dbContext;
    }
}

