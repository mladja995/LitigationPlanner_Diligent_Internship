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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using LitigationPlanner.Services;

namespace LitigationPlanner.Controllers
{
	[Authorize]
	public class LitigationController : Controller
    {
		private ILogger _logger = new Logger();
		private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Litigation
        public ActionResult Index(string sortOrder, string startSearchDate, string endSearchDate, string searchInstitution,
			string searchIdentifier, string searchCourtroom, string searchLocation, string searchProcedure,
			string searchJudge, string searchPlaintiff, string searchDefendant)
        {
			var litigations = ServiceLitigation.GetLitigations(db);
			PopulateDataForSorting(sortOrder);
			var litigationsFiltered = ServiceLitigation.Filter(litigations, startSearchDate, endSearchDate, searchInstitution,
											searchIdentifier, searchCourtroom, searchLocation, searchProcedure,
											searchJudge, searchPlaintiff, searchDefendant);
			var litigationSorted = ServiceLitigation.Sort(litigationsFiltered, sortOrder);
			return View(litigationSorted.ToList());
        }

        // GET: Litigation/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			Litigation litigation = ServiceLitigation.FindLitigation(id, db);
            if (litigation == null)
            {
                return HttpNotFound();
            }
            return View(litigation);
        }

		// GET: Litigation/Create
		[Authorize(Roles = "User")]
		public ActionResult Create()
        {
			PopulateSelectListsData(null);
			PopulateUsersData(null);
			return View();
        }

		
		// POST: Litigation/Create
		[HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "User")]
		public ActionResult Create([Bind(Include = "DateAndTimeOfMaintenance,TypeOfIstitution,ProcedureIdentifier," +
									"CourtroomNumber,Note,LocationID,JudgeID,PlaintiffID,DefendantID,TypeOfProcedureID")] Litigation litigation
									, string[] selectedUsers)
        {
			try
			{
				if (ModelState.IsValid)
				{
					var l = ServiceLitigation.AddEngagedLawers(litigation, selectedUsers, db);
					ServiceLitigation.AddLitigation(l, db);
					db.SaveChanges();
					return RedirectToAction("Index");
				}
			}
			catch (RetryLimitExceededException dex)
			{
				_logger.Error(dex.Message);
				ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
			}
			PopulateSelectListsData(litigation);
			return View(litigation);
        }

		// GET: Litigation/Edit/5
		[Authorize(Roles = "User")]
		public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			Litigation litigation = ServiceLitigation.FindLitigation(id, db);
            if (litigation == null)
            {
                return HttpNotFound();
            }
			PopulateSelectListsData(litigation);
			PopulateUsersData(litigation);
			return View(litigation);
		}

		// POST: Litigation/Edit/5
		[Authorize(Roles = "User")]
		[HttpPost, ActionName("Edit")]
		[ValidateAntiForgeryToken]
		public ActionResult EditPost(int? id, string[] selectedUsers)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var litigationToUpdate = ServiceLitigation.FindLitigation(id, db);
			var litigation = ServiceLitigation.UpdateEngagedLawyers(litigationToUpdate, selectedUsers, db);

			if (TryUpdateModel(litigation, "",
			   new string[] { "DateAndTimeOfMaintenance", "TypeOfIstitution", "ProcedureIdentifier", "CourtroomNumber", "Note",
								"LocationID", "JudgeID", "PlaintiffID", "DefendantID", "TypeOfProcedureID" }))
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
			PopulateSelectListsData(litigationToUpdate);
			return View(litigationToUpdate);
		}

		// GET: Litigation/Delete/5
		[Authorize(Roles = "User")]
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
			Litigation litigation = ServiceLitigation.FindLitigation(id, db);
            if (litigation == null)
            {
                return HttpNotFound();
            }
            return View(litigation);
        }

		// POST: Litigation/Delete/5
		[Authorize(Roles = "User")]
		[HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
			try
			{
				Litigation litigation = ServiceLitigation.FindLitigation(id, db);
				ServiceLitigation.RemoveLitigation(litigation, db);
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

		private void PopulateSelectListsData(Litigation litigation)
		{
			if (litigation == null)
			{
				ViewBag.DefendantID = new SelectList(ServiceContact.GetGontacts(db), "ID", "Name");
				ViewBag.JudgeID = new SelectList(ServiceContact.GetGontacts(db), "ID", "Name");
				ViewBag.PlaintiffID = new SelectList(ServiceContact.GetGontacts(db), "ID", "Name");
				ViewBag.LocationID = new SelectList(ServiceLocation.GetLocations(db), "ID", "Name");
				ViewBag.TypeOfProcedureID = new SelectList(ServiceProcedure.GetProcedures(db), "ID", "Name");
			}
			else
			{
				ViewBag.DefendantID = new SelectList(ServiceContact.GetGontacts(db), "ID", "Name", litigation.DefendantID);
				ViewBag.JudgeID = new SelectList(ServiceContact.GetGontacts(db), "ID", "Name", litigation.JudgeID);
				ViewBag.PlaintiffID = new SelectList(ServiceContact.GetGontacts(db), "ID", "Name", litigation.PlaintiffID);
				ViewBag.LocationID = new SelectList(ServiceLocation.GetLocations(db), "ID", "Name", litigation.LocationID);
				ViewBag.TypeOfProcedureID = new SelectList(ServiceProcedure.GetProcedures(db), "ID", "Name", litigation.TypeOfProcedureID);
			}

		}

		private void PopulateUsersData(Litigation litigation)
		{
			if(litigation != null)
			{
				List<String> engagedLawyers = new List<String>();
				foreach(var el in litigation.EngagedLawyers)
				{
					engagedLawyers.Add(el.Id);
				}

				ViewBag.EngagedLawyers = engagedLawyers;
			}
			var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

			var users = ServiceUser.GetUsers(db);

			List<ApplicationUser> usersWithRoleUser = new List<ApplicationUser>();
			
			foreach (var u in users.ToList())
			{
				if(UserManager.IsInRole(u.Id, "User"))
				{
					usersWithRoleUser.Add(u);
				}
			}

			ViewBag.Users = usersWithRoleUser;
		}

		private void PopulateDataForSorting(string sortOrder)
		{

			ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
			ViewBag.InstitutionSortParm = sortOrder == "Institution" ? "institution_desc" : "Institution";
			ViewBag.IdentifierSortParm = sortOrder == "Identifier" ? "identifier_desc" : "Identifier";
			ViewBag.CourtroomSortParm = sortOrder == "Courtroom" ? "courtroom_desc" : "Courtroom";
			ViewBag.LocationSortParm = sortOrder == "Location" ? "location_desc" : "Location";
			ViewBag.ProcedureSortParm = sortOrder == "Procedure" ? "procedure_desc" : "Procedure";
			ViewBag.JudgeSortParm = sortOrder == "Judge" ? "judge_desc" : "Judge";
			ViewBag.PlaintiffSortParm = sortOrder == "Plaintiff" ? "plaintiff_desc" : "Plaintiff";
			ViewBag.DefendantSortParm = sortOrder == "Defendant" ? "defendant_desc" : "Defendant";

		}

		#endregion

	}
}
