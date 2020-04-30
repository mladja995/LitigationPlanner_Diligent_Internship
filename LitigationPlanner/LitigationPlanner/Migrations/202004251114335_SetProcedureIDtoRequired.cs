namespace LitigationPlanner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetProcedureIDtoRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Litigation", "ProcedureIdentifier", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Litigation", "ProcedureIdentifier", c => c.String());
        }
    }
}
