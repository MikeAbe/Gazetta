namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tryingToRemoveCatMap : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Books", "BookCategory");
            DropColumn("dbo.Magazines", "MagazineCategory");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Magazines", "MagazineCategory", c => c.Int(nullable: false));
            AddColumn("dbo.Books", "BookCategory", c => c.Int(nullable: false));
        }
    }
}
