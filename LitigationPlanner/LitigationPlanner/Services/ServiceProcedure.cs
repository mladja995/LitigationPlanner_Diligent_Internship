using LitigationPlanner.DAL;
using LitigationPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LitigationPlanner.Services
{
	public static class ServiceProcedure
	{
		public static IEnumerable<TypeOfProcedure> Filter(IEnumerable<TypeOfProcedure> procedure, string searchString)
		{

			if (!String.IsNullOrEmpty(searchString))
			{
				procedure = procedure.Where(l => l.Name.Contains(searchString));
			}

			return procedure;
		}

		public static IEnumerable<TypeOfProcedure> Sort(IEnumerable<TypeOfProcedure> procedure, string sortOrder)
		{
			switch (sortOrder)
			{
				case "name_desc":
					procedure = procedure.OrderByDescending(l => l.Name);
					break;
				default:
					procedure = procedure.OrderBy(l => l.Name);
					break;
			}

			return procedure;
		}

		public static void DeleteConnections(TypeOfProcedure procedure, ApplicationDbContext db)
		{
			var litigations = db.Litigations;
			foreach (var litigation in litigations)
			{
				if (litigation.TypeOfProcedureID == procedure.ID)
					litigation.TypeOfProcedureID = null;
			}
		}

		public static IEnumerable<TypeOfProcedure> GetProcedures(ApplicationDbContext db)
		{
			var procedures = from p in db.TypeOfProcedures
							 select p;

			return procedures;
		}

		public static TypeOfProcedure FindProcedure(int? id, ApplicationDbContext db)
		{
			TypeOfProcedure procedure = db.TypeOfProcedures.Find(id);

			return procedure;
		}

		public static void AddProcedure(TypeOfProcedure typeOfProcedure, ApplicationDbContext db)
		{
			db.TypeOfProcedures.Add(typeOfProcedure);
		}
		
		public static void RemoveProcedure(TypeOfProcedure typeOfProcedure, ApplicationDbContext db)
		{
			db.TypeOfProcedures.Remove(typeOfProcedure);
		}
	}
}