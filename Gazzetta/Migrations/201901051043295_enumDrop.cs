namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class enumDrop : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "BookCategory", c => c.Int(nullable: false));
            AddColumn("dbo.Books", "BookCategoryName", c => c.String());
            AddColumn("dbo.Magazines", "MagazineCategory", c => c.Int(nullable: false));
            AddColumn("dbo.Magazines", "MagazineCategoryName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Magazines", "MagazineCategoryName");
            DropColumn("dbo.Magazines", "MagazineCategory");
            DropColumn("dbo.Books", "BookCategoryName");
            DropColumn("dbo.Books", "BookCategory");
        }
    }
}
