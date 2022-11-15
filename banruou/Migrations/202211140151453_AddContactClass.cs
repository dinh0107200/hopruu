namespace banruou.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddContactClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Fullname = c.String(maxLength: 100),
                        Address = c.String(maxLength: 200),
                        Mobile = c.String(nullable: false, maxLength: 20),
                        Email = c.String(maxLength: 100),
                        Body = c.String(maxLength: 4000),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Admins", "Role", c => c.Int(nullable: false));
            AddColumn("dbo.ProductCategories", "Body", c => c.String());
            CreateIndex("dbo.ArticleCategories", "ParentId");
            CreateIndex("dbo.ProductCategories", "ParentId");
            AddForeignKey("dbo.ArticleCategories", "ParentId", "dbo.ArticleCategories", "Id");
            AddForeignKey("dbo.ProductCategories", "ParentId", "dbo.ProductCategories", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductCategories", "ParentId", "dbo.ProductCategories");
            DropForeignKey("dbo.ArticleCategories", "ParentId", "dbo.ArticleCategories");
            DropIndex("dbo.ProductCategories", new[] { "ParentId" });
            DropIndex("dbo.ArticleCategories", new[] { "ParentId" });
            DropColumn("dbo.ProductCategories", "Body");
            DropColumn("dbo.Admins", "Role");
            DropTable("dbo.Contacts");
        }
    }
}
