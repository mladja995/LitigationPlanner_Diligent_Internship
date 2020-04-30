using LitigationPlanner.DAL;
using LitigationPlanner.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LitigationPlanner.Services
{
	public static class ServiceUser
	{
		public static IEnumerable<ApplicationUser> Filter(IEnumerable<ApplicationUser> users, string searchFirstName, string searchLastName, string searchEmail)
		{
			if (!String.IsNullOrEmpty(searchFirstName))
			{
				users = users.Where(u => u.FirstName.Contains(searchFirstName));
			}
			if (!String.IsNullOrEmpty(searchLastName))
			{
				users = users.Where(u => u.LastName.Contains(searchLastName));
			}
			if (!String.IsNullOrEmpty(searchEmail))
			{
				users = users.Where(u => u.Email.Contains(searchEmail));
			}
			
			return users;
		}

		public static IEnumerable<ApplicationUser> Sort(IEnumerable<ApplicationUser> users, string sortOrder)
		{
			switch (sortOrder)
			{
				case "firstName_desc":
					users = users.OrderByDescending(u => u.FirstName);
					break;
				case "LastName":
					users = users.OrderBy(u => u.LastName);
					break;
				case "lastName_desc":
					users = users.OrderByDescending(u => u.LastName);
					break;
				case "Email":
					users = users.OrderBy(u => u.Email);
					break;
				case "email_desc":
					users = users.OrderByDescending(u => u.Email);
					break;
				default:
					users = users.OrderBy(s => s.FirstName);
					break;
			}

			return users;
		}

		public static void UpdateUser(ApplicationUser applicationUser, string[] selectedRoles, ApplicationDbContext db)
		{
			var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
			var assignedRoles = userManager.GetRoles(applicationUser.Id).ToList();

			// Add new role
			foreach (var role in selectedRoles)
			{
				if (!assignedRoles.Contains(role))
				{
					userManager.AddToRole(applicationUser.Id, role);
				}
			}

			//Delete role
			foreach (var role in assignedRoles)
			{
				if (!selectedRoles.Contains(role))
				{
					userManager.RemoveFromRole(applicationUser.Id, role);
				}
			}

			var user = db.Users.Where(u => u.Id == applicationUser.Id).FirstOrDefault();
			user.Email = applicationUser.Email;
			user.UserName = applicationUser.Email;
			user.FirstName = applicationUser.FirstName;
			user.LastName = applicationUser.LastName;
		}

		public static IEnumerable<ApplicationUser> GetUsers(ApplicationDbContext db)
		{

			var users = from u in db.Users
						select u;

			return users;
		}

		public static ApplicationUser FindUser(string id, ApplicationDbContext db)
		{
			var user = db.Users.Find(id);

			return user;
		}
		
		public static void RemoveUser(ApplicationUser applicationUser, ApplicationDbContext db)
		{
			db.Users.Remove(applicationUser);
		}
		
	}
}