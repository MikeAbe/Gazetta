namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileSupport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserBooks", "Publication_Content", c => c.Binary());
            AddColumn("dbo.UserBooks", "Publication_FileType", c => c.Int(nullable: false));
            AddColumn("dbo.Magazines", "Publication_Content", c => c.Binary());
            AddColumn("dbo.Magazines", "Publication_FileType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Magazines", "Publication_FileType");
            DropColumn("dbo.Magazines", "Publication_Content");
            DropColumn("dbo.UserBooks", "Publication_FileType");
            DropColumn("dbo.UserBooks", "Publication_Content");
        }
    }
}
