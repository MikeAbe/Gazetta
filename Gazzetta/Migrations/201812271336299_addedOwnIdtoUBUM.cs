namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedOwnIdtoUBUM : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.UserBooks");
            DropPrimaryKey("dbo.UserMagazines");
            AddColumn("dbo.UserBooks", "UserBookId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.UserMagazines", "UserMagazineId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.UserBooks", "UserBookId");
            AddPrimaryKey("dbo.UserMagazines", "UserMagazineId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.UserMagazines");
            DropPrimaryKey("dbo.UserBooks");
            DropColumn("dbo.UserMagazines", "UserMagazineId");
            DropColumn("dbo.UserBooks", "UserBookId");
            AddPrimaryKey("dbo.UserMagazines", new[] { "ApplicationUserId", "MagazineId" });
            AddPrimaryKey("dbo.UserBooks", new[] { "AppliationUserId", "BookId" });
        }
    }
}
