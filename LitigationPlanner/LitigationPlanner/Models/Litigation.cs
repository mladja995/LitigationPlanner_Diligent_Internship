using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LitigationPlanner.Models
{
	public enum TypeOfIstitution
	{
		[Display(Name = "Viši sud")]
		VišiSud,

		[Display(Name = "Osnovni sud")]
		OsnovniSud,

		[Display(Name = "Privredni sud")]
		PrivredniSud,

		[Display(Name = "Prekršajni sud")]
		PrekršajniSud,

		[Display(Name = "Upravni sud")]
		UpravniSud
	}

	public class Litigation
	{
		public int ID { get; set; }

		[Display(Name = "Date and time")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
		public DateTime DateAndTimeOfMaintenance { get; set; }

		[Display(Name = "Institution")]
		public TypeOfIstitution TypeOfIstitution { get; set; }

		[Required]
		[Display(Name = "Identifier")]
		public string ProcedureIdentifier { get; set; }

		[Display(Name = "Courtroom")]
		public string CourtroomNumber { get; set; }

		public string Note { get; set; }

		[Display(Name = "Location")]
		public int? LocationID { get; set; }

		[Display(Name = "Procedure")]
		public int? TypeOfProcedureID { get; set; }

		[Display(Name = "Judge")]
		[ForeignKey("Judge")]
		public int? JudgeID { get; set; }

		[Display(Name = "Plaintiff")]
		[ForeignKey("Plaintiff")]
		public int? PlaintiffID { get; set; }

		[Display(Name = "Defendant")]
		[ForeignKey("Defendant")]
		public int? DefendantID { get; set; }

		public virtual Contact Judge { get; set; }
		public virtual Contact Plaintiff { get; set; }
		public virtual Contact Defendant { get; set; }
		public virtual Location Location { get; set; }
		public virtual TypeOfProcedure TypeOfProcedure { get; set; }

		[Display(Name = "Engaged lawyers")]
		public virtual ICollection<ApplicationUser> EngagedLawyers { get; set; }

		



	}
}