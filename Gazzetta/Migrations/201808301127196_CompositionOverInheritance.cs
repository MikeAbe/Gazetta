namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompositionOverInheritance : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Publications", "Price", c => c.Double(nullable: false));
            AlterColumn("dbo.UserBooks", "Name", c => c.String());
            DropColumn("dbo.Publications", "Author");
            DropColumn("dbo.Publications", "Blurb");
            DropColumn("dbo.Publications", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Publications", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Publications", "Blurb", c => c.String());
            AddColumn("dbo.Publications", "Author", c => c.String());
            AlterColumn("dbo.UserBooks", "Name", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Publications", "Price", c => c.Int(nullable: false));
        }
    }
}
