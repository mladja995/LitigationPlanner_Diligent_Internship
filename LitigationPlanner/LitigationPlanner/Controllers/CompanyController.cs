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
	public class CompanyController : Controller
    {
		private ILogger _logger = new Logger();
		private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Company
        public ActionResult Index(string sortOrder, string searchName, string searchAddress)
        {
			var companies = ServiceCompany.GetCompanies(db);
			PopulateDataForSorting(sortOrder);
			var companiesFiltered = ServiceCompany.Filter(companies, searchName, searchAddress);
			var companiessSorted = ServiceCompany.Sort(companiesFiltered, sortOrder);

			return View(companiessSorted.ToList());
		}

        // GET: Company/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			Company company = ServiceCompany.FindCompany(id, db);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // GET: Company/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Company/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Address")] Company company)
        {
			try
			{
				if (ModelState.IsValid)
				{
					ServiceCompany.AddCompany(company, db);
					db.SaveChanges();
					return RedirectToAction("Index");
				}
			}
			catch (RetryLimitExceededException dex)
			{
				_logger.Error(dex.Message);
				ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
			}

			return View(company);
        }

        // GET: Company/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			Company company = ServiceCompany.FindCompany(id, db);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

		// POST: Company/Edit/5
		[HttpPost, ActionName("Edit")]
		[ValidateAntiForgeryToken]
		public ActionResult EditPost(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var companyToUpdate = ServiceCompany.FindCompany(id, db);
			if (TryUpdateModel(companyToUpdate, "",
			   new string[] { "Name", "Address" }))
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
			return View(companyToUpdate);
		}

		// GET: Company/Delete/5
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
			Company company = ServiceCompany.FindCompany(id, db);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
			try
			{
				Company company = ServiceCompany.FindCompany(id, db);
				ServiceCompany.RemoveCompany(company, db);
				ServiceCompany.DeleteConnections(company, db);
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
			ViewBag.AddressSortParm = sortOrder == "Address" ? "address_desc" : "Address";

		}

		#endregion
	}
}
