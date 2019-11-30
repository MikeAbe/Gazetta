namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EpubSupportAdded : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserBooks", "Publication_Name", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.UserBooks", "Publication_Publisher", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.UserBooks", "Publication_Category", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.UserBooks", "Author", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Magazines", "Publication_Name", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Magazines", "Publication_Publisher", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Magazines", "Publication_Category", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Magazines", "Publication_Category", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Magazines", "Publication_Publisher", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.Magazines", "Publication_Name", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.UserBooks", "Author", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.UserBooks", "Publication_Category", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.UserBooks", "Publication_Publisher", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.UserBooks", "Publication_Name", c => c.String(nullable: false, maxLength: 30));
        }
    }
}
