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
	public class ContactController : Controller
    {
		private ILogger _logger = new Logger();
		private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Contact
        public ActionResult Index(string sortOrder, string searchName, string searchPhone,
			string searchEmail, string searchAddress, string searchOccupation, string searchCompany)
        {
			var contacts = ServiceContact.GetGontacts(db);
			PopulateDataForSorting(sortOrder);
			var contactsFiltered = ServiceContact.Filter(contacts, searchName, searchPhone,searchEmail, searchAddress, searchOccupation, searchCompany);
			var contactsSorted = ServiceContact.Sort(contactsFiltered, sortOrder);
			return View(contactsSorted.ToList());
		}

        // GET: Contact/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			Contact contact = ServiceContact.FindContact(id, db);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // GET: Contact/Create
        public ActionResult Create()
        {
            ViewBag.CompanyID = new SelectList(ServiceCompany.GetCompanies(db), "ID", "Name");
            return View();
        }

        // POST: Contact/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Phone1,Phone2,Address,Email,LegalEntity,Occupation,CompanyID")] Contact contact)
        {
			try
			{
				if (ModelState.IsValid)
				{
					ServiceContact.AddContact(contact, db);
					db.SaveChanges();
					return RedirectToAction("Index");
				}
			}
			catch (RetryLimitExceededException dex)
			{
				_logger.Error(dex.Message);
				ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
			}

			ViewBag.CompanyID = new SelectList(ServiceCompany.GetCompanies(db), "ID", "Name", contact.CompanyID);
            return View(contact);
        }

        // GET: Contact/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			Contact contact = ServiceContact.FindContact(id, db);
            if (contact == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyID = new SelectList(ServiceCompany.GetCompanies(db), "ID", "Name", contact.CompanyID);
            return View(contact);
		}

        // POST: Contact/Edit/5
        [HttpPost, ActionName("Edit")]
		[ValidateAntiForgeryToken]
		public ActionResult EditPost(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var contactToUpdate = ServiceContact.FindContact(id, db);
			if (TryUpdateModel(contactToUpdate, "",
			   new string[] { "Name", "Phone1", "Phone2", "Address", "Email", "LegalEntity", "Occupation", "CompanyID" }))
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
			return View(contactToUpdate);
		}

		// GET: Contact/Delete/5
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
			Contact contact = ServiceContact.FindContact(id, db);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: Contact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
			try
			{
				Contact contact = ServiceContact.FindContact(id, db);
				ServiceContact.RemoveContact(contact, db);
				ServiceContact.DeleteConnections(contact, db);
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
			ViewBag.OccupationSortParm = sortOrder == "Occupation" ? "occupation_desc" : "Occupation";
			ViewBag.CompanySortParm = sortOrder == "Company" ? "company_desc" : "Company";
			ViewBag.LegalEntitySortParm = sortOrder == "LegalEntity" ? "legalEntity_desc" : "LegalEntity";

		}

		#endregion
	}
}
