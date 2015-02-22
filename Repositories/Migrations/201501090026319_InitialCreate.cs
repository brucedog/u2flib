namespace Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuthenticationRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KeyHandle = c.String(),
                        Challenge = c.String(),
                        AppId = c.String(),
                        Version = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Devices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        KeyHandle = c.Binary(),
                        PublicKey = c.Binary(),
                        AttestationCert = c.Binary(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Password = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        AuthenticationRequest_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AuthenticationRequests", t => t.AuthenticationRequest_Id)
                .Index(t => t.AuthenticationRequest_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DeviceRegistrations", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Users", "AuthenticationRequest_Id", "dbo.AuthenticationRequests");
            DropIndex("dbo.Users", new[] { "AuthenticationRequest_Id" });
            DropIndex("dbo.DeviceRegistrations", new[] { "User_Id" });
            DropTable("dbo.Users");
            DropTable("dbo.DeviceRegistrations");
            DropTable("dbo.AuthenticationRequests");
        }
    }
}
