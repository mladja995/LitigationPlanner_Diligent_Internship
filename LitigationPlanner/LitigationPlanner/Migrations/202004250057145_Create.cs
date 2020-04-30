namespace LitigationPlanner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Company",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Address = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Contact",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Phone1 = c.Int(),
                        Phone2 = c.Int(),
                        Email = c.String(),
                        Address = c.String(),
                        LegalEntity = c.Boolean(nullable: false),
                        Occupation = c.String(),
                        CompanyID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Company", t => t.CompanyID)
                .Index(t => t.CompanyID);
            
            CreateTable(
                "dbo.Litigation",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DateAndTimeOfMaintenance = c.DateTime(nullable: false),
                        TypeOfIstitution = c.Int(nullable: false),
                        ProcedureIdentifier = c.String(),
                        CourtroomNumber = c.String(),
                        Note = c.String(),
                        LocationID = c.Int(),
                        TypeOfProcedureID = c.Int(),
                        JudgeID = c.Int(),
                        PlaintiffID = c.Int(),
                        DefendantID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Location", t => t.LocationID)
                .ForeignKey("dbo.TypeOfProcedure", t => t.TypeOfProcedureID)
                .ForeignKey("dbo.Contact", t => t.DefendantID)
                .ForeignKey("dbo.Contact", t => t.JudgeID)
                .ForeignKey("dbo.Contact", t => t.PlaintiffID)
                .Index(t => t.LocationID)
                .Index(t => t.TypeOfProcedureID)
                .Index(t => t.JudgeID)
                .Index(t => t.PlaintiffID)
                .Index(t => t.DefendantID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Location",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.TypeOfProcedure",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.ApplicationUserLitigation",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        Litigation_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.Litigation_ID })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Litigation", t => t.Litigation_ID, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.Litigation_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Litigation", "PlaintiffID", "dbo.Contact");
            DropForeignKey("dbo.Litigation", "JudgeID", "dbo.Contact");
            DropForeignKey("dbo.Litigation", "DefendantID", "dbo.Contact");
            DropForeignKey("dbo.Litigation", "TypeOfProcedureID", "dbo.TypeOfProcedure");
            DropForeignKey("dbo.Litigation", "LocationID", "dbo.Location");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserLitigation", "Litigation_ID", "dbo.Litigation");
            DropForeignKey("dbo.ApplicationUserLitigation", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Contact", "CompanyID", "dbo.Company");
            DropIndex("dbo.ApplicationUserLitigation", new[] { "Litigation_ID" });
            DropIndex("dbo.ApplicationUserLitigation", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Litigation", new[] { "DefendantID" });
            DropIndex("dbo.Litigation", new[] { "PlaintiffID" });
            DropIndex("dbo.Litigation", new[] { "JudgeID" });
            DropIndex("dbo.Litigation", new[] { "TypeOfProcedureID" });
            DropIndex("dbo.Litigation", new[] { "LocationID" });
            DropIndex("dbo.Contact", new[] { "CompanyID" });
            DropTable("dbo.ApplicationUserLitigation");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.TypeOfProcedure");
            DropTable("dbo.Location");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Litigation");
            DropTable("dbo.Contact");
            DropTable("dbo.Company");
        }
    }
}
