using LitigationPlanner.DAL;
using LitigationPlanner.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;


namespace LitigationPlanner.Services
{
	public static class ServiceLitigation
	{
		public static IEnumerable<Litigation> Filter(IEnumerable<Litigation> litigations, string startSearchDate, string endSearchDate, string searchInstitution,
			string searchIdentifier, string searchCourtroom, string searchLocation, string searchProcedure,
			string searchJudge, string searchPlaintiff, string searchDefendant)
		{

			if ((!String.IsNullOrEmpty(startSearchDate)) || (!String.IsNullOrEmpty(endSearchDate)))
			{
				if (String.IsNullOrEmpty(endSearchDate))
				{
					if (DateTime.TryParse(startSearchDate, out DateTime start))
					{
						litigations = litigations.Where(l => DbFunctions.TruncateTime(l.DateAndTimeOfMaintenance) >= start.Date);
					}
				}

				if (String.IsNullOrEmpty(startSearchDate))
				{
					if (DateTime.TryParse(endSearchDate, out DateTime end))
					{
						litigations = litigations.Where(l => DbFunctions.TruncateTime(l.DateAndTimeOfMaintenance) <= end.Date);
					}
				}

				if (DateTime.TryParse(startSearchDate, out DateTime startDt))
				{
					if (DateTime.TryParse(endSearchDate, out DateTime endDt))
					{
						litigations = litigations.Where(l => DbFunctions.TruncateTime(l.DateAndTimeOfMaintenance) >= startDt.Date
														&& DbFunctions.TruncateTime(l.DateAndTimeOfMaintenance) <= endDt.Date);
					}
				}
			}


			if (!String.IsNullOrEmpty(searchInstitution))
			{
				litigations = litigations.Where(l => l.TypeOfIstitution.ToString().Contains(searchInstitution));
			}
			if (!String.IsNullOrEmpty(searchIdentifier))
			{
				litigations = litigations.Where(l => l.ProcedureIdentifier.Contains(searchIdentifier));
			}
			if (!String.IsNullOrEmpty(searchCourtroom))
			{
				litigations = litigations.Where(l => l.CourtroomNumber.Contains(searchCourtroom));
			}
			if (!String.IsNullOrEmpty(searchLocation))
			{
				litigations = litigations.Where(l => l.Location.Name.Contains(searchLocation));
			}
			if (!String.IsNullOrEmpty(searchProcedure))
			{
				litigations = litigations.Where(l => l.TypeOfProcedure.Name.Contains(searchProcedure));
			}
			if (!String.IsNullOrEmpty(searchJudge))
			{
				litigations = litigations.Where(l => l.Judge.Name.Contains(searchJudge));
			}
			if (!String.IsNullOrEmpty(searchPlaintiff))
			{
				litigations = litigations.Where(l => l.Plaintiff.Name.Contains(searchPlaintiff));
			}
			if (!String.IsNullOrEmpty(searchDefendant))
			{
				litigations = litigations.Where(l => l.Defendant.Name.Contains(searchDefendant));
			}

			return litigations;
		}

		public static IEnumerable<Litigation> Sort(IEnumerable<Litigation> litigations, string sortOrder)
		{
			switch (sortOrder)
			{
				case "date_desc":
					litigations = litigations.OrderByDescending(l => l.DateAndTimeOfMaintenance);
					break;
				case "Institution":
					litigations = litigations.OrderBy(l => l.TypeOfIstitution);
					break;
				case "institution_desc":
					litigations = litigations.OrderByDescending(l => l.TypeOfIstitution);
					break;
				case "Identifier":
					litigations = litigations.OrderBy(l => l.ProcedureIdentifier);
					break;
				case "identifier_desc":
					litigations = litigations.OrderByDescending(l => l.ProcedureIdentifier);
					break;
				case "Courtroom":
					litigations = litigations.OrderBy(l => l.CourtroomNumber);
					break;
				case "courtroom_desc":
					litigations = litigations.OrderByDescending(l => l.CourtroomNumber);
					break;
				case "Location":
					litigations = litigations.OrderBy(l => l.Location?.Name);
					break;
				case "location_desc":
					litigations = litigations.OrderByDescending(l => l.Location?.Name);
					break;
				case "Procedure":
					litigations = litigations.OrderBy(l => l.TypeOfProcedure?.Name);
					break;
				case "procedure_desc":
					litigations = litigations.OrderByDescending(l => l.TypeOfProcedure?.Name);
					break;
				case "Judge":
					litigations = litigations.OrderBy(l => l.Judge?.Name);
					break;
				case "judge_desc":
					litigations = litigations.OrderByDescending(l => l.Judge?.Name);
					break;
				case "Plaintiff":
					litigations = litigations.OrderBy(l => l.Plaintiff?.Name);
					break;
				case "plaintiff_desc":
					litigations = litigations.OrderByDescending(l => l.Plaintiff?.Name);
					break;
				case "Defendant":
					litigations = litigations.OrderBy(l => l.Defendant?.Name);
					break;
				case "defendant_desc":
					litigations = litigations.OrderByDescending(l => l.Defendant?.Name);
					break;
				default:
					litigations = litigations.OrderBy(l => l.DateAndTimeOfMaintenance);
					break;
			}


			return litigations;
		}

		public static Litigation UpdateEngagedLawyers(Litigation litigation, string[] selectedUsers, ApplicationDbContext dbc)
		{
			var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbc));
			// Add new lawyer
			if (selectedUsers != null)
			{
				foreach (var userEmail in selectedUsers)
				{
					var user = userManager.FindByEmail(userEmail);
					if (!litigation.EngagedLawyers.Contains(user))
					{
						litigation.EngagedLawyers.Add(user);
					}
				}
			}
			List<ApplicationUser> engagedLawyers = litigation.EngagedLawyers.ToList();
			// Delete lawyer from litigation
			foreach (var user in engagedLawyers)
			{
				if (selectedUsers != null)
				{
					if (!selectedUsers.Contains(user.Email))
					{
						litigation.EngagedLawyers.Remove(user);
					}
				}
				else
				{
					litigation.EngagedLawyers.Remove(user);
				}
			}

			return litigation;
		}

		public static Litigation AddEngagedLawers(Litigation litigation, string[] selectedUsers, ApplicationDbContext db)
		{
			litigation.EngagedLawyers = new List<ApplicationUser>();
			var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
			if (selectedUsers != null)
			{
				foreach (var userEmail in selectedUsers)
				{
					var user = userManager.FindByEmail(userEmail);
					litigation.EngagedLawyers.Add(user);
				}
			}

			return litigation;
		}

		public static IEnumerable<Litigation> GetLitigations(ApplicationDbContext db)
		{

			var litigations = db.Litigations.Include(l => l.Defendant)
											.Include(l => l.Judge)
											.Include(l => l.Location)
											.Include(l => l.Plaintiff)
											.Include(l => l.TypeOfProcedure);

			return litigations;
		}

		public static Litigation FindLitigation(int? id, ApplicationDbContext dbc)
		{
			Litigation litigation = dbc.Litigations.Find(id);

			return litigation;
		}

		public static void AddLitigation(Litigation l, ApplicationDbContext db)
		{
			db.Litigations.Add(l);
		}
		
		public static void RemoveLitigation(Litigation litigation, ApplicationDbContext db)
		{
			db.Litigations.Remove(litigation);
		}

	}
}