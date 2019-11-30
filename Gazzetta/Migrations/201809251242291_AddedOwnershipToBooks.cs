namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOwnershipToBooks : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CustomerToMagazine", "CustomerId", "dbo.Magazines");
            DropForeignKey("dbo.CustomerToMagazine", "MagazineId", "dbo.UserBooks");
            DropForeignKey("dbo.CustomerToBook", "CustomerId", "dbo.UserBooks");
            DropForeignKey("dbo.CustomerToBook", "BookId", "dbo.UserBooks");
            DropIndex("dbo.CustomerToMagazine", new[] { "CustomerId" });
            DropIndex("dbo.CustomerToMagazine", new[] { "MagazineId" });
            DropIndex("dbo.CustomerToBook", new[] { "CustomerId" });
            DropIndex("dbo.CustomerToBook", new[] { "BookId" });
            CreateTable(
                "dbo.UsersToMagazines",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        MagazineId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.MagazineId })
                .ForeignKey("dbo.Magazines", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.MagazineId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.MagazineId);
            
            CreateTable(
                "dbo.CustomersToBooks",
                c => new
                    {
                        CustomerId = c.Int(nullable: false),
                        BookId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.CustomerId, t.BookId })
                .ForeignKey("dbo.UserBooks", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.BookId, cascadeDelete: true)
                .Index(t => t.CustomerId)
                .Index(t => t.BookId);
            
            AddColumn("dbo.UserBooks", "Owner_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUsers", "Customer_Id", c => c.Int());
            CreateIndex("dbo.UserBooks", "Owner_Id");
            CreateIndex("dbo.AspNetUsers", "Customer_Id");
            AddForeignKey("dbo.AspNetUsers", "Customer_Id", "dbo.UserBooks", "Id");
            AddForeignKey("dbo.UserBooks", "Owner_Id", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.UserBooks", "PhoneNumber");
            DropTable("dbo.CustomerToMagazine");
            DropTable("dbo.CustomerToBook");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CustomerToBook",
                c => new
                    {
                        CustomerId = c.Int(nullable: false),
                        BookId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CustomerId, t.BookId });
            
            CreateTable(
                "dbo.CustomerToMagazine",
                c => new
                    {
                        CustomerId = c.Int(nullable: false),
                        MagazineId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CustomerId, t.MagazineId });
            
            AddColumn("dbo.UserBooks", "PhoneNumber", c => c.String(nullable: false));
            DropForeignKey("dbo.UserBooks", "Owner_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CustomersToBooks", "BookId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CustomersToBooks", "CustomerId", "dbo.UserBooks");
            DropForeignKey("dbo.UsersToMagazines", "MagazineId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UsersToMagazines", "UserId", "dbo.Magazines");
            DropForeignKey("dbo.AspNetUsers", "Customer_Id", "dbo.UserBooks");
            DropIndex("dbo.CustomersToBooks", new[] { "BookId" });
            DropIndex("dbo.CustomersToBooks", new[] { "CustomerId" });
            DropIndex("dbo.UsersToMagazines", new[] { "MagazineId" });
            DropIndex("dbo.UsersToMagazines", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "Customer_Id" });
            DropIndex("dbo.UserBooks", new[] { "Owner_Id" });
            DropColumn("dbo.AspNetUsers", "Customer_Id");
            DropColumn("dbo.UserBooks", "Owner_Id");
            DropTable("dbo.CustomersToBooks");
            DropTable("dbo.UsersToMagazines");
            CreateIndex("dbo.CustomerToBook", "BookId");
            CreateIndex("dbo.CustomerToBook", "CustomerId");
            CreateIndex("dbo.CustomerToMagazine", "MagazineId");
            CreateIndex("dbo.CustomerToMagazine", "CustomerId");
            AddForeignKey("dbo.CustomerToBook", "BookId", "dbo.UserBooks", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CustomerToBook", "CustomerId", "dbo.UserBooks", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CustomerToMagazine", "MagazineId", "dbo.UserBooks", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CustomerToMagazine", "CustomerId", "dbo.Magazines", "Id", cascadeDelete: true);
        }
    }
}
