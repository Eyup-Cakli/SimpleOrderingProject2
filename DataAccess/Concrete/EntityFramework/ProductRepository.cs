﻿using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
using System.Threading.Tasks;
using System.Collections.Generic;
using Core.Entities.Dtos;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete.EntityFramework
{
    public class ProductRepository : EfEntityRepositoryBase<Product, ProjectDbContext>, IProductRepository
    {
        public ProductRepository(ProjectDbContext context) : base(context)
        {
        }

        public async Task<List<SelectionItem>> GetProductLookUp()
        {
            var lookUp = await (from entity in Context.Products
                                select new SelectionItem()
                                {
                                    Id = entity.Id,
                                    Label = entity.ProductName
                                }).ToListAsync();
            return lookUp;
        }

        public async Task<List<SelectionItem>> GetProductLookUpWithCode()
        {
            var lookUp = await (from entity in Context.Products
                               select new SelectionItem()
                               {
                                   Id = entity.Id,
                                   Label = entity.ProductName
                               }).ToListAsync();
            return lookUp;
        }
    }
}
