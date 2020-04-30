using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LitigationPlanner.Models
{
	public class TypeOfProcedure
	{
		public int ID { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 3)]
		public string Name { get; set; }

		public virtual ICollection<Litigation> Litigations { get; set; }
	}
}