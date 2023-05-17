
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

namespace DataAccess.Concrete.EntityFramework
{
    public class CustomerRepository : EfEntityRepositoryBase<Customer, ProjectDbContext>, ICustomerRepository
    {
        public CustomerRepository(ProjectDbContext context) : base(context)
        {
        }

        public async Task<List<SelectionItem>> GetCustomerLookUp()
        {
            var lookUp = await (from entity in Context.Customers
                                select new SelectionItem()
                                {
                                    Id = entity.Id,
                                    Label = entity.FirstName+" "+entity.LastName,
                                }).ToListAsync();
            return lookUp;
        }
    }
}
