namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testingEPUB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "Publication_MediaType", c => c.String());
            AddColumn("dbo.Magazines", "Publication_MediaType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Magazines", "Publication_MediaType");
            DropColumn("dbo.UserBooks", "Publication_MediaType");
        }
    }
}
