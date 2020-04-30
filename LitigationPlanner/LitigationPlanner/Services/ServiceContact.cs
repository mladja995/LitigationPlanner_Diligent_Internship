using LitigationPlanner.DAL;
using LitigationPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;

namespace LitigationPlanner.Services
{
	public static class ServiceContact 
	{
		public static IEnumerable<Contact> Filter(IEnumerable<Contact> contacts, string searchName, string searchPhone,
			string searchEmail, string searchAddress, string searchOccupation, string searchCompany)
		{
			if (!String.IsNullOrEmpty(searchName))
			{
				contacts = contacts.Where(c => c.Name.Contains(searchName));
			}
			if (!String.IsNullOrEmpty(searchPhone))
			{
				contacts = contacts.Where((c => c.Phone1.ToString().Contains(searchPhone) || c.Phone2.ToString().Contains(searchPhone)));
			}
			if (!String.IsNullOrEmpty(searchEmail))
			{
				contacts = contacts.Where(c => c.Email.Contains(searchEmail));
			}
			if (!String.IsNullOrEmpty(searchAddress))
			{
				contacts = contacts.Where(c => c.Address.Contains(searchAddress));
			}
			if (!String.IsNullOrEmpty(searchOccupation))
			{
				contacts = contacts.Where(c => c.Occupation.Contains(searchOccupation));
			}
			if (!String.IsNullOrEmpty(searchCompany))
			{
				contacts = contacts.Where(c => c.Company.Name.Contains(searchCompany));
			}

			
			return contacts;
		}

		public static IEnumerable<Contact> Sort(IEnumerable<Contact> contacts, string sortOrder)
		{
			
			switch (sortOrder)
			{
				case "name_desc":
					contacts = contacts.OrderByDescending(c => c.Name);
					break;
				case "Address":
					contacts = contacts.OrderBy(c => c.Address);
					break;
				case "address_desc":
					contacts = contacts.OrderByDescending(c => c.Address);
					break;
				case "Occupation":
					contacts = contacts.OrderBy(c => c.Occupation);
					break;
				case "occupation_desc":
					contacts = contacts.OrderByDescending(c => c.Occupation);
					break;
				case "Company":
					contacts = contacts.OrderBy(c => c.Company?.Name);
					break;
				case "company_desc":
					contacts = contacts.OrderByDescending(c => c.Company?.Name);
					break;
				case "LegalEntity":
					contacts = contacts.OrderBy(c => c.LegalEntity);
					break;
				case "legalEntity_desc":
					contacts = contacts.OrderByDescending(c => c.LegalEntity);
					break;
				default:
					contacts = contacts.OrderBy(c => c.Name);
					break;
			}

			return contacts;
		}

		public static void DeleteConnections(Contact contact, ApplicationDbContext db)
		{
			var litigations = db.Litigations;
			foreach (var litigation in litigations)
			{
				if (litigation.JudgeID == contact.ID)
					litigation.JudgeID = null;

				if (litigation.PlaintiffID == contact.ID)
					litigation.PlaintiffID = null;

				if (litigation.DefendantID == contact.ID)
					litigation.DefendantID = null;
			}
		}

		public static IEnumerable<Contact> GetGontacts(ApplicationDbContext db)
		{
			var contacts = db.Contacts.Include(c => c.Company);

			return contacts;
		}

		public static Contact FindContact(int? id, ApplicationDbContext db)
		{
			Contact contact = db.Contacts.Find(id);

			return contact;
		}

		public static void AddContact(Contact contact, ApplicationDbContext db)
		{
			db.Contacts.Add(contact);
		}
		
		public static void RemoveContact(Contact contact, ApplicationDbContext db)
		{
			db.Contacts.Remove(contact);
		}
	}
}