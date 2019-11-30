namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MagazineSupportAdded : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Magazines", "FrequencyOfPublication");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Magazines", "FrequencyOfPublication", c => c.Byte(nullable: false));
        }
    }
}
