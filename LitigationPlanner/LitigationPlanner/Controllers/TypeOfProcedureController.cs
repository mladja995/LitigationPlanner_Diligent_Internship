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
	public class TypeOfProcedureController : Controller
    {
		private ILogger _logger = new Logger();
		private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TypeOfProcedure
        public ActionResult Index(string sortOrder, string searchString)
        {
			var procedures = ServiceProcedure.GetProcedures(db);
			PopulateDataForSorting(sortOrder);
			var proceduresFiltered = ServiceProcedure.Filter(procedures, searchString);
			var proceduresSorted = ServiceProcedure.Sort(proceduresFiltered, sortOrder);

			return View(proceduresSorted.ToList());
		}

        // GET: TypeOfProcedure/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			TypeOfProcedure typeOfProcedure = ServiceProcedure.FindProcedure(id, db);
            if (typeOfProcedure == null)
            {
                return HttpNotFound();
            }
            return View(typeOfProcedure);
        }

        // GET: TypeOfProcedure/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TypeOfProcedure/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name")] TypeOfProcedure typeOfProcedure)
        {
			try
			{
				if (ModelState.IsValid)
				{
					ServiceProcedure.AddProcedure(typeOfProcedure, db);
					db.SaveChanges();
					return RedirectToAction("Index");
				}
			}
			catch (RetryLimitExceededException dex)
			{
				_logger.Error(dex.Message);
				ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
			}
			return View(typeOfProcedure);
        }

        // GET: TypeOfProcedure/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			TypeOfProcedure typeOfProcedure = ServiceProcedure.FindProcedure(id, db);

			if (typeOfProcedure == null)
            {
                return HttpNotFound();
            }
            return View(typeOfProcedure);
        }

		// POST: TypeOfProcedure/Edit/5
		[HttpPost, ActionName("Edit")]
		[ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var procedureToUpdate = ServiceProcedure.FindProcedure(id, db);
			if (TryUpdateModel(procedureToUpdate, "",
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
			return View(procedureToUpdate);
        }

        // GET: TypeOfProcedure/Delete/5
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
			TypeOfProcedure typeOfProcedure = ServiceProcedure.FindProcedure(id, db);
            if (typeOfProcedure == null)
            {
                return HttpNotFound();
            }
            return View(typeOfProcedure);
        }

        // POST: TypeOfProcedure/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
			try
			{
				TypeOfProcedure typeOfProcedure = ServiceProcedure.FindProcedure(id, db);
				ServiceProcedure.RemoveProcedure(typeOfProcedure, db);
				ServiceProcedure.DeleteConnections(typeOfProcedure, db);
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
