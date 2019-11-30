namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.UsersToMagazines", newName: "CustomersToMagazines");
            RenameColumn(table: "dbo.CustomersToMagazines", name: "UserId", newName: "CustomerId");
            RenameIndex(table: "dbo.CustomersToMagazines", name: "IX_UserId", newName: "IX_CustomerId");
            AddColumn("dbo.Magazines", "Owner_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Magazines", "Owner_Id");
            AddForeignKey("dbo.Magazines", "Owner_Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Magazines", "Owner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Magazines", new[] { "Owner_Id" });
            DropColumn("dbo.Magazines", "Owner_Id");
            RenameIndex(table: "dbo.CustomersToMagazines", name: "IX_CustomerId", newName: "IX_UserId");
            RenameColumn(table: "dbo.CustomersToMagazines", name: "CustomerId", newName: "UserId");
            RenameTable(name: "dbo.CustomersToMagazines", newName: "UsersToMagazines");
        }
    }
}
