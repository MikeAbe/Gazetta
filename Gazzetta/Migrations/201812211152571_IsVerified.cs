namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsVerified : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsVerrified", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserBooks", "Hits", c => c.Byte(nullable: false));
            AddColumn("dbo.UserMagazines", "Hits", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserMagazines", "Hits");
            DropColumn("dbo.UserBooks", "Hits");
            DropColumn("dbo.AspNetUsers", "IsVerrified");
        }
    }
}
