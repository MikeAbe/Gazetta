namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class customerToPublication : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserBooks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Email = c.String(),
                        PhoneNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Publications", "Customers_Id", c => c.Int());
            CreateIndex("dbo.Publications", "Customers_Id");
            AddForeignKey("dbo.Publications", "Customers_Id", "dbo.UserBooks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Publications", "Customers_Id", "dbo.UserBooks");
            DropIndex("dbo.Publications", new[] { "Customers_Id" });
            DropColumn("dbo.Publications", "Customers_Id");
            DropTable("dbo.UserBooks");
        }
    }
}
