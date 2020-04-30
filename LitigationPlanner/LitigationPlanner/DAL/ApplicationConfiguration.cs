using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;

namespace LitigationPlanner.DAL
{
	public class ApplicationConfiguration : DbConfiguration
	{
		public ApplicationConfiguration()
		{
			SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
		}
	}
}