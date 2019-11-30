namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Polished : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserBooks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Publication_Name = c.String(nullable: false, maxLength: 50),
                        Publication_Price = c.Double(nullable: false),
                        Publication_Publisher = c.String(nullable: false, maxLength: 50),
                        Publication_Category = c.String(nullable: false, maxLength: 50),
                        Publication_Language = c.String(nullable: false, maxLength: 20),
                        Publication_Description = c.String(nullable: false, maxLength: 280),
                        Author = c.String(nullable: false, maxLength: 50),
                        Blurb = c.String(maxLength: 280),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserBooks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 20),
                        Email = c.String(nullable: false, maxLength: 30),
                        PhoneNumber = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Magazines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Publication_Name = c.String(nullable: false, maxLength: 30),
                        Publication_Price = c.Double(nullable: false),
                        Publication_Publisher = c.String(nullable: false, maxLength: 30),
                        Publication_Category = c.String(nullable: false, maxLength: 20),
                        Publication_Language = c.String(nullable: false, maxLength: 20),
                        Publication_Description = c.String(nullable: false, maxLength: 280),
                        FrequencyOfPublication = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CustomerToMagazine",
                c => new
                    {
                        CustomerId = c.Int(nullable: false),
                        MagazineId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CustomerId, t.MagazineId })
                .ForeignKey("dbo.Magazines", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("dbo.UserBooks", t => t.MagazineId, cascadeDelete: true)
                .Index(t => t.CustomerId)
                .Index(t => t.MagazineId);
            
            CreateTable(
                "dbo.CustomerToBook",
                c => new
                    {
                        CustomerId = c.Int(nullable: false),
                        BookId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CustomerId, t.BookId })
                .ForeignKey("dbo.UserBooks", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("dbo.UserBooks", t => t.BookId, cascadeDelete: true)
                .Index(t => t.CustomerId)
                .Index(t => t.BookId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerToBook", "BookId", "dbo.UserBooks");
            DropForeignKey("dbo.CustomerToBook", "CustomerId", "dbo.UserBooks");
            DropForeignKey("dbo.CustomerToMagazine", "MagazineId", "dbo.UserBooks");
            DropForeignKey("dbo.CustomerToMagazine", "CustomerId", "dbo.Magazines");
            DropIndex("dbo.CustomerToBook", new[] { "BookId" });
            DropIndex("dbo.CustomerToBook", new[] { "CustomerId" });
            DropIndex("dbo.CustomerToMagazine", new[] { "MagazineId" });
            DropIndex("dbo.CustomerToMagazine", new[] { "CustomerId" });
            DropTable("dbo.CustomerToBook");
            DropTable("dbo.CustomerToMagazine");
            DropTable("dbo.Magazines");
            DropTable("dbo.UserBooks");
            DropTable("dbo.UserBooks");
        }
    }
}
