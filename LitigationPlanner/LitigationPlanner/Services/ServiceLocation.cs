using LitigationPlanner.DAL;
using LitigationPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LitigationPlanner.Services
{
	public static class ServiceLocation
	{
		public static IEnumerable<Location> Filter(IEnumerable<Location> locations, string searchString)
		{
			if (!String.IsNullOrEmpty(searchString))
			{
				locations = locations.Where(l => l.Name.Contains(searchString));
			}

			return locations;
		}

		public static IEnumerable<Location> Sort(IEnumerable<Location> locations, string sortOrder)
		{
			switch (sortOrder)
			{
				case "name_desc":
					locations = locations.OrderByDescending(l => l.Name);
					break;
				default:
					locations = locations.OrderBy(l => l.Name);
					break;
			}

			return locations;
		}

		public static void DeleteConnections(Location location, ApplicationDbContext db)
		{
			var litigations = db.Litigations;
			foreach (var litigation in litigations)
			{
				if (litigation.LocationID == location.ID)
					litigation.LocationID = null;
			}
		}

		public static IEnumerable<Location> GetLocations(ApplicationDbContext db)
		{
			var locations = from l in db.Locations
							select l;

			return locations;
		}

		public static Location FindLocation(int? id, ApplicationDbContext db)
		{
			Location location = db.Locations.Find(id);

			return location;
		}

		public static void AddLocation(Location location, ApplicationDbContext db)
		{
			db.Locations.Add(location);
		}
		
		public static void RemoveLocation(Location location, ApplicationDbContext db)
		{
			db.Locations.Remove(location);
		}
		
	}
}