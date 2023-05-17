
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DataAccess;
using Core.Entities.Dtos;
using Entities.Concrete;
using Entities.Dtos;

namespace DataAccess.Abstract
{
    public interface IOrderRepository : IEntityRepository<Order>
    {
        Task<List<SelectionItem>> GetOrderLookUp();
        Task<List<OrderDto>> GetOrderDto();
        Task<List<OrderDto>> getOrderListDtoByDate(string startDate, string endDate);
    }
}