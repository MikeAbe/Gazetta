namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class workingThumnail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserBooks", "Publication_Thumbnail", c => c.Binary());
            AddColumn("dbo.Magazines", "Publication_Thumbnail", c => c.Binary());
            AlterColumn("dbo.UserBooks", "Publication_Content", c => c.Binary(nullable: false));
            AlterColumn("dbo.Magazines", "Publication_Content", c => c.Binary(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Magazines", "Publication_Content", c => c.Binary());
            AlterColumn("dbo.UserBooks", "Publication_Content", c => c.Binary());
            DropColumn("dbo.Magazines", "Publication_Thumbnail");
            DropColumn("dbo.UserBooks", "Publication_Thumbnail");
        }
    }
}
