namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PayableandTags : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "Publication_Tags", c => c.String());
            AddColumn("dbo.Books", "Publication_Downloaded", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Payable", c => c.Double(nullable: false));
            AddColumn("dbo.Magazines", "Publication_Tags", c => c.String());
            AddColumn("dbo.Magazines", "Publication_Downloaded", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Magazines", "Publication_Downloaded");
            DropColumn("dbo.Magazines", "Publication_Tags");
            DropColumn("dbo.AspNetUsers", "Payable");
            DropColumn("dbo.Books", "Publication_Downloaded");
            DropColumn("dbo.Books", "Publication_Tags");
        }
    }
}
