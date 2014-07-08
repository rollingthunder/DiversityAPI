namespace DiversityService.API.WebHost.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CollectionId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CollectionId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "CollectionId");
        }
    }
}
