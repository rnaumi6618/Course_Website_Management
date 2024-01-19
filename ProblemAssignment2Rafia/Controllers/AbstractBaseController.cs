using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProblemAssignment2Rafia.Controllers
{
    public abstract class AbstractBaseController : Controller
    {
       
        private const string FirstVisitCookieName = "FirstVisit";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewBag.UserTrackingMessage = GenerateUserTrackingMessage("Controller");
        }
        protected string GenerateUserTrackingMessage(string pageName)
        {
            // Check if the "FirstVisit" cookie exists
            if (Request.Cookies[FirstVisitCookieName] == null)
            {
                // If not, create the cookie with the current timestamp
                //var firstVisitTime = DateTime.UtcNow.ToString("f");
                var utcNow = DateTime.UtcNow;
                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                var firstVisitTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, easternZone).ToString("f");
                Response.Cookies.Append(FirstVisitCookieName, firstVisitTime);

                // Welcome the user on their first visit
                return "Hey!Welcome to the Course Manager App";
            }
            else
            {
                // If the cookie exists, retrieve the timestamp
                string firstVisitTime = Request.Cookies[FirstVisitCookieName];
                return $"Welcome back! You first started using the app on {firstVisitTime}.";
            }
        }
    }
}
