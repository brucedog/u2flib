namespace Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MultiDeviesForUsers : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Users", new[] { "AuthenticationRequest_Id" });
            DropForeignKey("dbo.Users", "FK_dbo.Users_dbo.AuthenticationRequests_AuthenticationRequest_Id");
            AddColumn("dbo.AuthenticationRequests", "User_Id", c => c.Int());
            AddForeignKey("dbo.AuthenticationRequests", "User_Id", "dbo.Users");
            CreateIndex("dbo.AuthenticationRequests", "User_Id");
            DropColumn("dbo.Users", "AuthenticationRequest_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "AuthenticationRequest_Id", c => c.Int());
            DropIndex("dbo.AuthenticationRequests", "User_Id");
            DropForeignKey("dbo.AuthenticationRequests", "FK_dbo.Users_dbo.AuthenticationRequests_User_Id");
            DropColumn("dbo.AuthenticationRequests", "User_Id");
            AddForeignKey("dbo.Users", "AuthenticationRequest_Id", "dbo.AuthenticationRequests");
            CreateIndex("dbo.Users", "AuthenticationRequest_Id");
        }
    }
}
