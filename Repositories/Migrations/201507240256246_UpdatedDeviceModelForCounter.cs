namespace Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedDeviceModelForCounter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Devices", "Counter", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Devices", "Counter");
        }
    }
}
