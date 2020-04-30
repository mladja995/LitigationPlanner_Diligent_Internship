using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LitigationPlanner.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using LitigationPlanner.DAL;
using System.Data.Entity.Infrastructure;
using LitigationPlanner.Logging;
using LitigationPlanner.Services;

namespace LitigationPlanner.Controllers
{
	[Authorize(Roles = "Admin")]
	public class ApplicationUserController : Controller
    {
		private ILogger _logger = new Logger();
		private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ApplicationUser
        public ActionResult Index(string sortOrder, string searchFirstName, string searchLastName, string searchEmail)
        {
			var users = ServiceUser.GetUsers(db);
			PopulateDataForSorting(sortOrder);
			var usersFiltered = ServiceUser.Filter(users, searchFirstName, searchLastName, searchEmail);
			var usersSorted = ServiceUser.Sort(usersFiltered, sortOrder);
			return View(usersSorted.ToList());

        }

        // GET: ApplicationUser/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			ApplicationUser applicationUser = ServiceUser.FindUser(id,db);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
			ViewBag.Litigations = applicationUser.Litigations.ToList();
			PopulateRolesData(id);
			return View(applicationUser);
        }

		// GET: ApplicationUser/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = ServiceUser.FindUser(id,db);
			if (applicationUser == null)
            {
                return HttpNotFound();
            }
			PopulateRolesData(id);
            return View(applicationUser);
        }

		// POST: ApplicationUser/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id, FirstName,LastName,Email")] ApplicationUser applicationUser, string[] selectedRoles)
        {
			try
			{
				if (ModelState.IsValid)
				{
					if (selectedRoles == null)
					{
						ModelState.AddModelError(string.Empty, "You must select at least one role.");
						PopulateRolesData(applicationUser.Id);
						return View(applicationUser);
					}

					ServiceUser.UpdateUser(applicationUser, selectedRoles, db);
					db.SaveChanges();
					return RedirectToAction("Index");

				}
			}
			catch (RetryLimitExceededException dex)
			{
				_logger.Error(dex.Message);
				ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
			}


			return View(applicationUser);
        }

        // GET: ApplicationUser/Delete/5
        public ActionResult Delete(string id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			if (saveChangesError.GetValueOrDefault())
			{
				ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
			}
			ApplicationUser applicationUser = ServiceUser.FindUser(id,db);
			if (applicationUser == null)
            {
                return HttpNotFound();
            }

			PopulateRolesData(id);
			return View(applicationUser);
        }

        // POST: ApplicationUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
			try
			{
				ApplicationUser applicationUser = ServiceUser.FindUser(id, db);
				ServiceUser.RemoveUser(applicationUser, db);
				db.SaveChanges();
			}
			catch (RetryLimitExceededException dex)
			{
				_logger.Error(dex.Message);
				return RedirectToAction("Delete", new { id = id, saveChangesError = true });
			}
			return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

		#region Helpers

		private void PopulateRolesData(string id)
		{
			var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
			var assignedRoles = userManager.GetRoles(id).ToList();
			ViewBag.AssignedRoles = assignedRoles;

			var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
			var allRoles = roleManager.Roles.ToList();
			ViewBag.AllRoles = allRoles;
		}

		private void PopulateDataForSorting(string sortOrder)
		{
			ViewBag.FirstNameSortParm = String.IsNullOrEmpty(sortOrder) ? "firstName_desc" : "";
			ViewBag.LastNameSortParm = sortOrder == "LastName" ? "lastName_desc" : "LastName";
			ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";

		}

		
		#endregion
	}
}
