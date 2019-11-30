namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modified : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Publications", "Customers_Id", "dbo.UserBooks");
            DropIndex("dbo.Publications", new[] { "Customers_Id" });
            DropTable("dbo.Publications");
            DropTable("dbo.UserBooks");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserBooks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        PhoneNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Publications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Price = c.Double(nullable: false),
                        Publisher = c.String(),
                        Category = c.String(),
                        Language = c.String(),
                        Description = c.String(),
                        Customers_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.Publications", "Customers_Id");
            AddForeignKey("dbo.Publications", "Customers_Id", "dbo.UserBooks", "Id");
        }
    }
}
