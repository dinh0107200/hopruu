namespace banruou.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateConfigSite : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConfigSites", "ProductNote", c => c.String());
            AlterColumn("dbo.ConfigSites", "Zalo", c => c.String(maxLength: 50));
            AlterColumn("dbo.ConfigSites", "Image", c => c.String(maxLength: 500));
            AlterColumn("dbo.ConfigSites", "Favicon", c => c.String(maxLength: 500));
            AlterColumn("dbo.ConfigSites", "AboutImage", c => c.String(maxLength: 500));
            AlterColumn("dbo.Products", "Price", c => c.Int());
            DropColumn("dbo.ConfigSites", "Address2");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ConfigSites", "Address2", c => c.String(maxLength: 500));
            AlterColumn("dbo.Products", "Price", c => c.Int(nullable: false));
            AlterColumn("dbo.ConfigSites", "AboutImage", c => c.String());
            AlterColumn("dbo.ConfigSites", "Favicon", c => c.String());
            AlterColumn("dbo.ConfigSites", "Image", c => c.String());
            AlterColumn("dbo.ConfigSites", "Zalo", c => c.String(maxLength: 500));
            DropColumn("dbo.ConfigSites", "ProductNote");
        }
    }
}
