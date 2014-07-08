namespace DiversityService.API.WebHost.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BackendCredentials : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "BackendUser", c => c.String(maxLength: 4000));
            AddColumn("dbo.AspNetUsers", "BackendPassword", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "BackendPassword");
            DropColumn("dbo.AspNetUsers", "BackendUser");
        }
    }
}
