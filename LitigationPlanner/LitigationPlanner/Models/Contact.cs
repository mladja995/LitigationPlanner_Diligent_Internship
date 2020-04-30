using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LitigationPlanner.Models
{
	public class Contact
	{
		public int ID { get; set; }

		[Required]
		[StringLength(50, MinimumLength = 3)]
		public string Name { get; set; }

		[DataType(DataType.PhoneNumber)]
		public int? Phone1 { get; set; }

		[DataType(DataType.PhoneNumber)]
		public int? Phone2 { get; set; }

		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		public string Address { get; set; }

		[Display(Name = "Legal entity")]
		public bool LegalEntity { get; set; }

		public string Occupation { get; set; }

		[Display(Name = "Company")]
		public int? CompanyID { get; set; }

		public virtual Company Company { get; set; }

		[InverseProperty("Judge")]
		public virtual ICollection<Litigation> LitigationsForJudges { get; set; }

		[InverseProperty("Plaintiff")]
		public virtual ICollection<Litigation> LitigationsForPlaintiffs { get; set; }

		[InverseProperty("Defendant")]
		public virtual ICollection<Litigation> LitigationsForDefendants { get; set; }

	}
}