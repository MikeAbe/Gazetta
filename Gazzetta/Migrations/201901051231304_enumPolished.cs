namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class enumPolished : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Books", "BookCategoryName");
            DropColumn("dbo.Magazines", "MagazineCategoryName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Magazines", "MagazineCategoryName", c => c.String());
            AddColumn("dbo.Books", "BookCategoryName", c => c.String());
        }
    }
}
