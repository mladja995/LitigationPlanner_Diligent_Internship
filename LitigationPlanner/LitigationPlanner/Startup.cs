using LitigationPlanner.DAL;
using LitigationPlanner.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LitigationPlanner.Startup))]
namespace LitigationPlanner
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
			CreateRolesandUsers();
		}

		// In this method we will create default User roles and Admin user for login    
		private void CreateRolesandUsers()
		{
			ApplicationDbContext db = new ApplicationDbContext();

			var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
			var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

			// Creating User role     
			if (!roleManager.RoleExists("User"))
			{
				var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
				role.Name = "User";
				roleManager.Create(role);

			}

			// In Startup I'm creating first Admin Role and creating a default Admin User     
			if (!roleManager.RoleExists("Admin"))
			{
				// First we create Admin role    
				var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
				role.Name = "Admin";
				roleManager.Create(role);

				// Here we create a Admin super user who will maintain the website                   
				var user = new ApplicationUser();
				user.UserName = "mladen.mladenovic@example.com";
				user.Email = "mladen.mladenovic@example.com";
				user.FirstName = "Mladen";
				user.LastName = "Mladenović";

				string userPWD = "Mladen@95";

				var chkUser = UserManager.Create(user, userPWD);
			

				// Add default User to Role Admin    
				if (chkUser.Succeeded)
				{
					var result1 = UserManager.AddToRole(user.Id, "Admin");
					var result2 = UserManager.AddToRole(user.Id, "User");

				}
			}
		}
	}
}
