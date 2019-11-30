namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nameAdded : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "Customer_Id", "dbo.UserBooks");
            DropIndex("dbo.AspNetUsers", new[] { "Customer_Id" });
            AddColumn("dbo.AspNetUsers", "Name", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.AspNetUsers", "Customer_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Customer_Id", c => c.Int());
            DropColumn("dbo.AspNetUsers", "Name");
            CreateIndex("dbo.AspNetUsers", "Customer_Id");
            AddForeignKey("dbo.AspNetUsers", "Customer_Id", "dbo.UserBooks", "Id");
        }
    }
}
