﻿using banruou.Models;
using System.Data.Entity;
namespace banruou.DAL
{
    public class DataEntities : DbContext
    {
        public DataEntities() : base("name=DataEntities") { }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ConfigSite> ConfigSites { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleCategory> ArticleCategories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}