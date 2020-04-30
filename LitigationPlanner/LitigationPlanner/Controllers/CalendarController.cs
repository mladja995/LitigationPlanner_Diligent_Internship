using LitigationPlanner.DAL;
using LitigationPlanner.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LitigationPlanner.Controllers
{
	[Authorize(Roles = "User")]
	public class CalendarController : Controller
    {
		private ApplicationDbContext db = new ApplicationDbContext();

		// GET: Calendar
		public ActionResult Index()
        {
            return View();
        }

		public JsonResult GetEvents()
		{
			//var events = dc.Events.ToList();
			var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
			var currentUser = userManager.FindById(User.Identity.GetUserId());
			var litigations = currentUser.Litigations.ToList();
			List<Event> events = new List<Event>();
			
			foreach(var l in litigations)
			{
				Event e = new Event
				{
					Title = l.ProcedureIdentifier,
					Description = l.Note,
					Date = l.DateAndTimeOfMaintenance
				};

				events.Add(e);
			}

			return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
			
		}
	}
}