namespace banruou.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPropertyOfProducts : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Products", "ProductCategoryId", "dbo.ProductCategories");
            DropIndex("dbo.Products", new[] { "ProductCategoryId" });
            CreateTable(
                "dbo.GroupPropertyOfProducts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Sort = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PropertyOfProducts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Sort = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PropertyAndProductDetails",
                c => new
                    {
                        ProductId = c.Int(nullable: false),
                        PropertyOfProductId = c.Int(nullable: false),
                        Name = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => new { t.ProductId, t.PropertyOfProductId })
                .ForeignKey("dbo.PropertyOfProducts", t => t.PropertyOfProductId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.PropertyOfProductId);
            
            CreateTable(
                "dbo.PropertyOfProductGroupPropertyOfProducts",
                c => new
                    {
                        PropertyOfProduct_Id = c.Int(nullable: false),
                        GroupPropertyOfProduct_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PropertyOfProduct_Id, t.GroupPropertyOfProduct_Id })
                .ForeignKey("dbo.PropertyOfProducts", t => t.PropertyOfProduct_Id, cascadeDelete: true)
                .ForeignKey("dbo.GroupPropertyOfProducts", t => t.GroupPropertyOfProduct_Id, cascadeDelete: true)
                .Index(t => t.PropertyOfProduct_Id)
                .Index(t => t.GroupPropertyOfProduct_Id);
            
            CreateTable(
                "dbo.ProductProductCategories",
                c => new
                    {
                        Product_Id = c.Int(nullable: false),
                        ProductCategory_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Product_Id, t.ProductCategory_Id })
                .ForeignKey("dbo.Products", t => t.Product_Id, cascadeDelete: true)
                .ForeignKey("dbo.ProductCategories", t => t.ProductCategory_Id, cascadeDelete: true)
                .Index(t => t.Product_Id)
                .Index(t => t.ProductCategory_Id);
            
            AddColumn("dbo.ProductCategories", "GroupPropertyOfProductId", c => c.Int());
            CreateIndex("dbo.ProductCategories", "GroupPropertyOfProductId");
            AddForeignKey("dbo.ProductCategories", "GroupPropertyOfProductId", "dbo.GroupPropertyOfProducts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PropertyAndProductDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.PropertyAndProductDetails", "PropertyOfProductId", "dbo.PropertyOfProducts");
            DropForeignKey("dbo.ProductProductCategories", "ProductCategory_Id", "dbo.ProductCategories");
            DropForeignKey("dbo.ProductProductCategories", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.ProductCategories", "GroupPropertyOfProductId", "dbo.GroupPropertyOfProducts");
            DropForeignKey("dbo.PropertyOfProductGroupPropertyOfProducts", "GroupPropertyOfProduct_Id", "dbo.GroupPropertyOfProducts");
            DropForeignKey("dbo.PropertyOfProductGroupPropertyOfProducts", "PropertyOfProduct_Id", "dbo.PropertyOfProducts");
            DropIndex("dbo.ProductProductCategories", new[] { "ProductCategory_Id" });
            DropIndex("dbo.ProductProductCategories", new[] { "Product_Id" });
            DropIndex("dbo.PropertyOfProductGroupPropertyOfProducts", new[] { "GroupPropertyOfProduct_Id" });
            DropIndex("dbo.PropertyOfProductGroupPropertyOfProducts", new[] { "PropertyOfProduct_Id" });
            DropIndex("dbo.PropertyAndProductDetails", new[] { "PropertyOfProductId" });
            DropIndex("dbo.PropertyAndProductDetails", new[] { "ProductId" });
            DropIndex("dbo.ProductCategories", new[] { "GroupPropertyOfProductId" });
            DropColumn("dbo.ProductCategories", "GroupPropertyOfProductId");
            DropTable("dbo.ProductProductCategories");
            DropTable("dbo.PropertyOfProductGroupPropertyOfProducts");
            DropTable("dbo.PropertyAndProductDetails");
            DropTable("dbo.PropertyOfProducts");
            DropTable("dbo.GroupPropertyOfProducts");
            CreateIndex("dbo.Products", "ProductCategoryId");
            AddForeignKey("dbo.Products", "ProductCategoryId", "dbo.ProductCategories", "Id", cascadeDelete: true);
        }
    }
}
