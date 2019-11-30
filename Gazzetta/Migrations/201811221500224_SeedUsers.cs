namespace Gazzetta.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedUsers : DbMigration
    {
        public override void Up()
        {
           Sql(@"
INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [Customer_Id]) VALUES (N'19ed2eb0-65e5-4897-9813-4764991e4579', N'Guest@Gazetta.com', 0, N'AJe+lX1+T5TTm+7zr67tUMhDvVOdXT2x/hvRs85wfq2GJ26Qk4O5I5HZe8YF512tiw==', N'd2decf0e-8ce0-4f9a-9710-fb8045f6713e', N'+251912628405', 0, 0, NULL, 1, 0, N'Guest@Gazetta.com', NULL)
INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [Customer_Id]) VALUES (N'e4f3f81c-1bb9-4f51-b2b2-0a3b9655c0d6', N'Administrator@Gazetta.com', 0, N'AIpEaV5BvLv2HouzBm9vp0S9YjtS26oy1aMZ1exL82T+LkTwDV8Gk4sxF25caO/p8g==', N'76758fb7-2071-4c39-b850-37e7d58e0028', N'+251912628405', 0, 0, NULL, 1, 0, N'Administrator@Gazetta.com', NULL)


INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'bfc76995-78bb-4f20-9ff0-e5d768c1b69c', N'CanDoAnything')

INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'e4f3f81c-1bb9-4f51-b2b2-0a3b9655c0d6', N'bfc76995-78bb-4f20-9ff0-e5d768c1b69c')




");
        }
        
        public override void Down()
        {
        }
    }
}
