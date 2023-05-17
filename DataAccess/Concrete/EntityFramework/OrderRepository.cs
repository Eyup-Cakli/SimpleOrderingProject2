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
    public class OrderRepository : EfEntityRepositoryBase<Order, ProjectDbContext>, IOrderRepository
    {
        public OrderRepository(ProjectDbContext context) : base(context)
        {
        }

        public async Task<List<OrderDto>> GetOrderDto()
        {
            var list = await (from order in Context.Orders
                              join product in Context.Products on order.ProductId equals product.Id //into joinedProduct
                              //from product in joinedProduct.DefaultIfEmpty()
                              join customer in Context.Customers on order.CustomerId equals customer.Id //into joinedCustomer
                              //from customer in joinedCustomer.DefaultIfEmpty()
                              where order.isDeleted == false
                              select new OrderDto()
                              {
                                  Id = order.Id,
                                  CustomerId = customer.Id,
                                  CustomerName = customer.FirstName + " "+customer.LastName,
                                  ProductId = product.Id,
                                  ProductName = product.ProductName,
                                  Color = product.Color,
                                  Quantity = order.Quantity,
                                  Size = product.Size,
                                  Status = order.Status,
                                  isDeleted = order.isDeleted,
                                  CreatedUserId = order.CreatedUserId,
                                  CreatedDate = order.CreatedDate,
                                  LastUpdatedUserId = order.LastUpdatedUserId,
                                  LastUpdatedDate = order.LastUpdatedDate
                              }).ToListAsync();
            return list;
        }

        public async Task<List<OrderDto>> getOrderListDtoByDate(string startDate, string endDate)
        {
            DateTime StartDateFormat = DateTime.ParseExact(startDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            DateTime EndDateFormat = DateTime.ParseExact(endDate, "dd.MM.yyyy", CultureInfo.InvariantCulture).AddDays(1);

            var list = await (from order in Context.Orders
                              join product in Context.Products on order.ProductId equals product.Id
                              join customer in Context.Customers on order.CustomerId equals customer.Id
                              where order.isDeleted == false
                              && order.CreatedDate >= StartDateFormat && order.CreatedDate <= EndDateFormat
                              select new OrderDto()
                              {
                                  Id = order.Id,
                                  CustomerId = customer.Id,
                                  CustomerName = customer.FirstName + " " + customer.LastName,
                                  ProductId = product.Id,
                                  ProductName = product.ProductName,
                                  Color = product.Color,
                                  Quantity = order.Quantity,
                                  Size = product.Size,
                                  Status = order.Status,
                                  isDeleted = order.isDeleted,
                                  CreatedUserId = order.CreatedUserId,
                                  CreatedDate = order.CreatedDate,
                                  LastUpdatedUserId = order.LastUpdatedUserId,
                                  LastUpdatedDate = order.LastUpdatedDate
                              }).ToListAsync();
            return list;
        }

        public async Task<List<SelectionItem>> GetOrderLookUp()
        {
            var lookup = await (from entity in Context.Orders
                                select new SelectionItem()
                                {
                                    Id = entity.Id,
                                    Label = entity.Id.ToString()
                                }).ToListAsync();
            return lookup;
        }
    }
}
