namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Issue2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Magazines", "IssueNumber", c => c.DateTime(nullable: false, precision: 0, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Magazines", "IssueNumber", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
    }
}
