
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
using System.Threading.Tasks;
using System.Collections.Generic;
using Core.Entities.Dtos;
using Microsoft.EntityFrameworkCore;
using Entities.Dtos;
using System.Globalization;

namespace DataAccess.Concrete.EntityFramework
{
    public class WarehouseRepository : EfEntityRepositoryBase<Warehouse, ProjectDbContext>, IWarehouseRepository
    {
        public WarehouseRepository(ProjectDbContext context) : base(context)
        {
        }

        public async Task<List<WarehouseDto>> GetWarehouseDtoByDate(string startDate, string endDate )
        {
            DateTime startDateFormat = DateTime.ParseExact( startDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            DateTime endDateFormat = DateTime.ParseExact( endDate, "dd.MM.yyyy", CultureInfo.InvariantCulture).AddDays(1);

            var list = await (from warehouse in Context.Warehouses
                              join product in Context.Products on warehouse.ProductId equals product.Id into warehouseProduct
                              from product in warehouseProduct.DefaultIfEmpty()
                              join user in Context.Users on warehouse.CreatedUserId equals user.UserId into userProduct
                              from user in userProduct.DefaultIfEmpty()
                              where product.isDeleted == false
                              && warehouse.CreatedDate >= startDateFormat && warehouse.CreatedDate <= endDateFormat

                              select new WarehouseDto()
                              {
                                  Id = warehouse.Id,
                                  ProductId = product.Id,
                                  Size = product.Size,
                                  ProductName = product.ProductName,
                                  Color = product.Color,
                                  Quantity = warehouse.Quantity,
                                  Status = warehouse.Status,
                                  isDeleted = warehouse.isDeleted,
                                  CreatedDate = warehouse.CreatedDate,
                                  CreatedUserId = warehouse.CreatedUserId,
                                  UserName = user.FullName,
                                  ReadyForSale = warehouse.ReadyForSale,
                                  LastUpdatedDate = warehouse.LastUpdatedDate,
                                  LastUpdatedUserId = warehouse.LastUpdatedUserId
                              }).ToListAsync();
            return list;
        }

        public async Task<List<WarehouseDto>> GetWarehouseDto()
        {
            var list = await (from warehouse in Context.Warehouses
                              join product in Context.Products on warehouse.ProductId equals product.Id
                              join user in Context.Users on warehouse.CreatedUserId equals user.UserId
                              where product.isDeleted == false

                              select new WarehouseDto()
                              {
                                  Id = warehouse.Id,
                                  ProductId = product.Id,
                                  Size = product.Size,
                                  ProductName = product.ProductName,
                                  Color = product.Color,
                                  Quantity = warehouse.Quantity,
                                  Status = warehouse.Status,
                                  isDeleted = warehouse.isDeleted,
                                  CreatedDate = warehouse.CreatedDate,
                                  CreatedUserId = warehouse.CreatedUserId,
                                  UserName = user.FullName,
                                  ReadyForSale = warehouse.ReadyForSale,
                                  LastUpdatedDate = warehouse.LastUpdatedDate,
                                  LastUpdatedUserId = warehouse.LastUpdatedUserId
                              }).ToListAsync();
            return list;
        }

        public async Task<List<SelectionItem>> GetWarehouseLookUp()
        {
            var lookUp = await (from entity in Context.Warehouses select new SelectionItem() 
            {
                Id = entity.ProductId
            }).ToListAsync();
            return lookUp;
        }

        public async Task<List<SelectionItem>> GetWarehouseLookUpWithCode()
        {
            var lookUp = await (from entity in Context.Warehouses
                                select new SelectionItem()
                                {
                                    Id = entity.ProductId
                                }).ToListAsync();
            return lookUp;
        }

        public async Task<bool> SynchronOrder(int productId, int quantity, string readyForSale, bool isDeleted)
        {
            var warehouse = await Context.Warehouses.AnyAsync(x => x.ProductId == productId && x.Quantity >= quantity && x.ReadyForSale == "ready" && x.isDeleted == false);
            return warehouse;
        }
    }
}
