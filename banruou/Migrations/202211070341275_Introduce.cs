namespace banruou.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Introduce : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ConfigSites", "Introduce", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ConfigSites", "Introduce");
        }
    }
}
