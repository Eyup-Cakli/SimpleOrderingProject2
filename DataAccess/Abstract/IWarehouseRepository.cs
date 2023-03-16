
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DataAccess;
using Core.Entities.Dtos;
using Entities.Concrete;
namespace DataAccess.Abstract
{
    public interface IWarehouseRepository : IEntityRepository<Warehouse>
    {
        Task<List<SelectionItem>> GetWarehouseLookUp();
        Task<List<SelectionItem>> GetWarehouseLookUpWithCode();
    }
}