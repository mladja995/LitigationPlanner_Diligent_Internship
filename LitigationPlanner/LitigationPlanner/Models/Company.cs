using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LitigationPlanner.Models
{
	public class Company
	{
		public int ID { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 2)]
		public string Name { get; set; }

		[Required]
		[StringLength(50)]
		public string Address { get; set; }

		public ICollection<Contact> Contacts { get; set; }
	}
}