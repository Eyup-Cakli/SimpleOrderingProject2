using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class WarehouseDto : BaseEntityDto, IDto
    {
        
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Color { get; set; }
        public string UserName { get; set; }
        public int Quantity { get; set; }
        public string ReadyForSale { get; set; }
        public string Size { get; set; }
    }
}
