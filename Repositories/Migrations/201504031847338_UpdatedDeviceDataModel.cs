namespace Repositories.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedDeviceDataModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Devices", "IsCompromised", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Devices", "IsCompromised");
        }
    }
}
