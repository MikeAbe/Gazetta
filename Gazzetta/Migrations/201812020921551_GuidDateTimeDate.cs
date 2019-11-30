namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GuidDateTimeDate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserMagazines", "MagazineId", "dbo.Magazines");
            DropIndex("dbo.UserMagazines", new[] { "MagazineId" });
            DropPrimaryKey("dbo.UserMagazines");
            DropPrimaryKey("dbo.Magazines");
            DropColumn("dbo.Magazines","Id");
            DropColumn("dbo.UserMagazines","MagazineId");
            AddColumn("dbo.UserMagazines", "MagazineId", c => c.Guid(nullable: false));
            AddColumn("dbo.Magazines", "Id", c => c.Guid(nullable: false, identity: true,defaultValueSql:"newsequentialid()"));
            AlterColumn("dbo.Magazines", "IssueNumber", c => c.DateTime(nullable: false, storeType: "date"));
            AddPrimaryKey("dbo.UserMagazines", new[] { "ApplicationUserId", "MagazineId" });
            AddPrimaryKey("dbo.Magazines", "Id");
            CreateIndex("dbo.UserMagazines", "MagazineId");
            AddForeignKey("dbo.UserMagazines", "MagazineId", "dbo.Magazines", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserMagazines", "MagazineId", "dbo.Magazines");
            DropIndex("dbo.UserMagazines", new[] { "MagazineId" });
            DropPrimaryKey("dbo.Magazines");
            DropPrimaryKey("dbo.UserMagazines");
            AlterColumn("dbo.Magazines", "IssueNumber", c => c.DateTime(nullable: false, precision: 0, storeType: "datetime2"));
            AlterColumn("dbo.Magazines", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.UserMagazines", "MagazineId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Magazines", "Id");
            AddPrimaryKey("dbo.UserMagazines", new[] { "ApplicationUserId", "MagazineId" });
            CreateIndex("dbo.UserMagazines", "MagazineId");
            AddForeignKey("dbo.UserMagazines", "MagazineId", "dbo.Magazines", "Id", cascadeDelete: true);
        }
    }
}
