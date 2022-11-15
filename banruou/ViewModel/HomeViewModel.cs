using banruou.Models;
using PagedList;
using System.Collections.Generic;

namespace banruou.ViewModel
{
    public class HomeViewModel
    {
        public IEnumerable<Article> NewArticles { get; set; }
        public IEnumerable<Banner> Banners { get; set; }
        public IEnumerable<HomeBlock> HomeBlocks { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }

        public class HomeBlock
        {
            public ProductCategory Category { get; set; }
            public IEnumerable<Product> Products { get; set; }
        }
    }
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
    public class AllProductViewModel
    {
        public PagedList.IPagedList<Product> Products { get; set; }
        //public IEnumerable<Product> Products { get; set; }
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
        public int ProductCount { get; set; }
        public int CountCategory { get; set; }
    }
    public class ProductViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
        public int ProductCount { get; set; }
        public int CountCategory { get; set; }
    }
    public class GetAllProductViewModel
    {
        public IPagedList<Product> Products { get; set; }
        public int? CatId { get; set; }
    }
    public class AllArticleViewModel
    {
        public IPagedList<Article> Articles { get; set; }
        public int ArticleCount { get; set; }
    }
    public class ArticleDetailsViewModel
    {
        public Article Article { get; set; }
        public IEnumerable<Article> Articles { get; set; }
    }
    public class ArticleCategoryViewModel
    {
        public ArticleCategory RootCategory { get; set; }
        public ArticleCategory Category { get; set; }
        public IPagedList<Article> Articles { get; set; }
        public int ArticleCount { get; set; }
    }
    public class CategoryProduct
    {
        public PagedList.IPagedList<Product> Products { get; set; }
        public ProductCategory ProductCategory { get; set; }
        //public IEnumerable<Product> Products { get; set; }
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
        public int ProductCount { get; set; }
        public int CountCategory { get; set; }
    }
    public class ShowArtilceCategory
    {
        public ArticleCategory ArticleCategory { get; set; }
        public IEnumerable<Article> Articles { get; set; }
        public IEnumerable<ArticleCategory> ArticleCategories { get; set; }

    }
    public class GetNavMenu
    {
        public IEnumerable<ArticleCategory> ArticleCategories { get; set; }
        public IEnumerable<ArticleCategory> IntroduceCat { get; set; }
    }
    public class GetNavProduct
    {
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
        public int CategoryId { get; set; }
    }


    public class PluginViewModel
    {
        public IEnumerable<ConfigSite> configSites { get; set; }
    }
    public class HeaderViewModel
    {
        public IEnumerable<ArticleCategory> ArticleCategories { get; set; }
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
    }
    public class FooterViewModel
    {
        public IEnumerable<ArticleCategory> ArticleCategories { get; set; }
        public IEnumerable<ArticleCategory> Introduce { get; set; }
        public IEnumerable<ArticleCategory> Policies { get; set; }
        public IEnumerable<ProductCategory> FooterProduct { get; set; }
    }
    public class OrderViewModel
    {
        public Order Order { get; set; }
        public Product Products { get; set; }
    }
}