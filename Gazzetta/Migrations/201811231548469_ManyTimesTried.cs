namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ManyTimesTried : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CustomersToMagazines", "CustomerId", "dbo.Magazines");
            DropForeignKey("dbo.CustomersToMagazines", "MagazineId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CustomersToBooks", "CustomerId", "dbo.Books");
            DropForeignKey("dbo.CustomersToBooks", "BookId", "dbo.AspNetUsers");
            DropIndex("dbo.CustomersToMagazines", new[] { "CustomerId" });
            DropIndex("dbo.CustomersToMagazines", new[] { "MagazineId" });
            DropIndex("dbo.CustomersToBooks", new[] { "CustomerId" });
            DropIndex("dbo.CustomersToBooks", new[] { "BookId" });
            CreateTable(
                "dbo.UserBooks",
                c => new
                    {
                        AppliationUserId = c.String(nullable: false, maxLength: 128),
                        BookId = c.Int(nullable: false),
                        PurchaseDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.AppliationUserId, t.BookId })
                .ForeignKey("dbo.AspNetUsers", t => t.AppliationUserId, cascadeDelete: true)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .Index(t => t.AppliationUserId)
                .Index(t => t.BookId);
            
            CreateTable(
                "dbo.UserMagazines",
                c => new
                    {
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        MagazineId = c.Int(nullable: false),
                        PurchaseDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUserId, t.MagazineId })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .ForeignKey("dbo.Magazines", t => t.MagazineId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.MagazineId);
            
            AlterColumn("dbo.Customers", "Name", c => c.String());
            AlterColumn("dbo.Customers", "Email", c => c.String());
            DropTable("dbo.CustomersToMagazines");
            DropTable("dbo.CustomersToBooks");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CustomersToBooks",
                c => new
                    {
                        CustomerId = c.Int(nullable: false),
                        BookId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.CustomerId, t.BookId });
            
            CreateTable(
                "dbo.CustomersToMagazines",
                c => new
                    {
                        CustomerId = c.Int(nullable: false),
                        MagazineId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.CustomerId, t.MagazineId });
            
            DropForeignKey("dbo.UserMagazines", "MagazineId", "dbo.Magazines");
            DropForeignKey("dbo.UserMagazines", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserBooks", "BookId", "dbo.Books");
            DropForeignKey("dbo.UserBooks", "AppliationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserMagazines", new[] { "MagazineId" });
            DropIndex("dbo.UserMagazines", new[] { "ApplicationUserId" });
            DropIndex("dbo.UserBooks", new[] { "BookId" });
            DropIndex("dbo.UserBooks", new[] { "AppliationUserId" });
            AlterColumn("dbo.Customers", "Email", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.Customers", "Name", c => c.String(nullable: false, maxLength: 20));
            DropTable("dbo.UserMagazines");
            DropTable("dbo.UserBooks");
            CreateIndex("dbo.CustomersToBooks", "BookId");
            CreateIndex("dbo.CustomersToBooks", "CustomerId");
            CreateIndex("dbo.CustomersToMagazines", "MagazineId");
            CreateIndex("dbo.CustomersToMagazines", "CustomerId");
            AddForeignKey("dbo.CustomersToBooks", "BookId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CustomersToBooks", "CustomerId", "dbo.Books", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CustomersToMagazines", "MagazineId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CustomersToMagazines", "CustomerId", "dbo.Magazines", "Id", cascadeDelete: true);
        }
    }
}
