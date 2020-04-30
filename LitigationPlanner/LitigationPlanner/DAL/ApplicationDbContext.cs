using LitigationPlanner.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace LitigationPlanner.DAL
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext()
			: base("LitigationPlannerDB1", throwIfV1Schema: false)
		{
		}

		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
		}

		public System.Data.Entity.DbSet<LitigationPlanner.Models.Location> Locations { get; set; }

		public System.Data.Entity.DbSet<LitigationPlanner.Models.TypeOfProcedure> TypeOfProcedures { get; set; }

		public System.Data.Entity.DbSet<LitigationPlanner.Models.Company> Companies { get; set; }

		public System.Data.Entity.DbSet<LitigationPlanner.Models.Contact> Contacts { get; set; }

		public System.Data.Entity.DbSet<LitigationPlanner.Models.Litigation> Litigations { get; set; }

	}
}