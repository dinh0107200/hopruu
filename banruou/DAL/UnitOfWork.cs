﻿using banruou.Models;
using System;

namespace banruou.DAL
{
    public class UnitOfWork : IDisposable
    {
        private readonly DataEntities _context = new DataEntities();
        private GenericRepository<Admin> _adminRepository;
        private GenericRepository<ConfigSite> _configRepository;
        private GenericRepository<Question> _QuestionRepository;
        private GenericRepository<Banner> _BannerRepository;
        private GenericRepository<ProductCategory> _ProductCategoryRepository;
        private GenericRepository<Product> _ProductRepository;
        private GenericRepository<ArticleCategory> _articleCategoryRepository;
        private GenericRepository<Article> _ArticleRepository;
        private GenericRepository<Order> _OrderRepository;
        private GenericRepository<Contact> _contactRepository;
        private GenericRepository<PropertyOfProduct> _propertyOfProductRepository;
        private GenericRepository<GroupPropertyOfProduct> _groupPropertyOfProductRepository;
        private GenericRepository<PropertyAndProductDetail> _propertyAndProductDetailRepository;

        public GenericRepository<PropertyAndProductDetail> PropertyAndProductDetailRepository => _propertyAndProductDetailRepository ?? (_propertyAndProductDetailRepository = new GenericRepository<PropertyAndProductDetail>(_context));
        public GenericRepository<GroupPropertyOfProduct> GroupPropertyOfProductRepository => _groupPropertyOfProductRepository ?? (_groupPropertyOfProductRepository = new GenericRepository<GroupPropertyOfProduct>(_context));
        public GenericRepository<PropertyOfProduct> PropertyOfProductRepository => _propertyOfProductRepository ?? (_propertyOfProductRepository = new GenericRepository<PropertyOfProduct>(_context));
        public GenericRepository<Contact> ContactRepository =>
            _contactRepository ?? (_contactRepository = new GenericRepository<Contact>(_context));
        public GenericRepository<Admin> AdminRepository =>
            _adminRepository ?? (_adminRepository = new GenericRepository<Admin>(_context));
        public GenericRepository<ConfigSite> ConfigSiteRepository =>
            _configRepository ?? (_configRepository = new GenericRepository<ConfigSite>(_context));
        public GenericRepository<Question> QuestionRepository =>
            _QuestionRepository ?? (_QuestionRepository = new GenericRepository<Question>(_context));
        public GenericRepository<Banner> BannerRepository =>
           _BannerRepository ?? (_BannerRepository = new GenericRepository<Banner>(_context));
        public GenericRepository<ProductCategory> ProductCategoryRepository =>
            _ProductCategoryRepository ?? (_ProductCategoryRepository = new GenericRepository<ProductCategory>(_context));
        public GenericRepository<Product> ProductRepository =>
         _ProductRepository ?? (_ProductRepository = new GenericRepository<Product>(_context));
        public GenericRepository<ArticleCategory> ArticleCategoryRepository =>
            _articleCategoryRepository ?? (_articleCategoryRepository = new GenericRepository<ArticleCategory>(_context));
        public GenericRepository<Article> ArticleRepository =>
            _ArticleRepository ?? (_ArticleRepository = new GenericRepository<Article>(_context));
        public GenericRepository<Order> OrderRepository => _OrderRepository ?? (_OrderRepository = new GenericRepository<Order>(_context));

        public void Save()
        {
            _context.SaveChanges();
        }
        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}