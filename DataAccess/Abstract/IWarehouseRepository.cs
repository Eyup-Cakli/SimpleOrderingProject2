using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DataAccess;
using Core.Entities.Dtos;
using Entities.Concrete;
using Entities.Dtos;

namespace DataAccess.Abstract
{
    public interface IWarehouseRepository : IEntityRepository<Warehouse>
    {
        Task<List<SelectionItem>> GetWarehouseLookUp();
        Task<List<SelectionItem>> GetWarehouseLookUpWithCode();
        Task<List<WarehouseDto>> GetWarehouseDto();
        Task<List<WarehouseDto>> GetWarehouseDtoByDate(string startDate, string endtime);
        Task<bool> SynchronOrder(int productId, int quantity, string readyForSale, bool isDeleted);
    }
}