using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LitigationPlanner.DAL;
using LitigationPlanner.Models;
using System.Data.Entity.Infrastructure;
using LitigationPlanner.Logging;
using LitigationPlanner.Services;

namespace LitigationPlanner.Controllers
{
	[Authorize(Roles = "Admin")]
	public class LocationController : Controller
    {
		private ApplicationDbContext db = new ApplicationDbContext();
		private ILogger _logger = new Logger();
		
        // GET: Location
        public ActionResult Index(string sortOrder, string searchString)
        {

			var locations = ServiceLocation.GetLocations(db);
			PopulateDataForSorting(sortOrder);
			var locationsFiltered = ServiceLocation.Filter(locations, searchString);
			var locationsSorted = ServiceLocation.Sort(locationsFiltered, sortOrder);
			
			return View(locationsSorted.ToList());
        }

        // GET: Location/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			Location location = ServiceLocation.FindLocation(id, db);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // GET: Location/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Location/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name")] Location location)
        {
			try
			{
				if (ModelState.IsValid)
				{
					ServiceLocation.AddLocation(location, db);
					db.SaveChanges();
					return RedirectToAction("Index");
				}
			}
			catch (RetryLimitExceededException dex)
			{
				_logger.Error(dex.Message);
				ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
			}
			return View(location);
        }

        // GET: Location/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			Location location = ServiceLocation.FindLocation(id, db);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

		// POST: Location/Edit/5
		[HttpPost, ActionName("Edit")]
		[ValidateAntiForgeryToken]
		public ActionResult EditPost(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var locationToUpdate = ServiceLocation.FindLocation(id, db);
			if (TryUpdateModel(locationToUpdate, "",
			   new string[] { "Name" }))
			{
				try
				{
					db.SaveChanges();

					return RedirectToAction("Index");
				}
				catch (RetryLimitExceededException dex)
				{
					_logger.Error(dex.Message);
					ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				}
			}
			return View(locationToUpdate);
		}

		// GET: Location/Delete/5
		public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			if (saveChangesError.GetValueOrDefault())
			{
				ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
			}
			Location location = ServiceLocation.FindLocation(id, db);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // POST: Location/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
			try
			{
				Location location = ServiceLocation.FindLocation(id, db);
				ServiceLocation.RemoveLocation(location, db);
				ServiceLocation.DeleteConnections(location, db);
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
		private void PopulateDataForSorting(string sortOrder)
		{

			ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

		}
		#endregion
	}
}
