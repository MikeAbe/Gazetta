namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class statusOfPurchaseAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserBooks", "Status", c => c.String());
            AddColumn("dbo.UserMagazines", "Status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMagazines", "Status");
            DropColumn("dbo.UserBooks", "Status");
        }
    }
}
