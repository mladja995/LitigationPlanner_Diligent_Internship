using LitigationPlanner.DAL;
using LitigationPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LitigationPlanner.Services
{
	public static class ServiceCompany
	{
		public static IEnumerable<Company> Filter(IEnumerable<Company> companies, string searchName, string searchAddress)
		{
			if (!String.IsNullOrEmpty(searchName))
			{
				companies = companies.Where(c => c.Name.Contains(searchName));
			}
			if (!String.IsNullOrEmpty(searchAddress))
			{
				companies = companies.Where(c => c.Address.Contains(searchAddress));
			}
			return companies;
		}

		public static IEnumerable<Company> Sort(IEnumerable<Company> companies, string sortOrder)
		{
			switch (sortOrder)
			{
				case "name_desc":
					companies = companies.OrderByDescending(c => c.Name);
					break;
				case "Address":
					companies = companies.OrderBy(c => c.Address);
					break;
				case "address_desc":
					companies = companies.OrderByDescending(c => c.Address);
					break;
				default:
					companies = companies.OrderBy(c => c.Name);
					break;
			}

			return companies;
		}

		public static void DeleteConnections(Company company, ApplicationDbContext db)
		{
			var contacts = db.Contacts;
			foreach (var contact in contacts)
			{
				if (contact.CompanyID == company.ID)
					contact.CompanyID = null;
			}
		}

		public static IEnumerable<Company> GetCompanies(ApplicationDbContext db)
		{
			var companies = from c in db.Companies
							select c;

			return companies;
		}

		public static Company FindCompany(int? id, ApplicationDbContext db)
		{
			Company company = db.Companies.Find(id);

			return company;
		}

		public static void AddCompany(Company company, ApplicationDbContext db)
		{
			db.Companies.Add(company);
		}
		
		public static void RemoveCompany(Company company, ApplicationDbContext db)
		{
			db.Companies.Remove(company);
		}
	}
}