using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Warehouses.Queries
{
    public class GetWarehouseByProductIdAndSizeQuery : IRequest<IDataResult<Warehouse>>
    {
        public int ProductId { get; set; }
        public string Size { get; set; }

        public class GetWarehouseByProductIdAndSizeQueryHandler : IRequestHandler<GetWarehouseByProductIdAndSizeQuery, IDataResult<Warehouse>>
        {
            private readonly IWarehouseRepository _wareHouseRepository;
            private readonly IMediator _mediator;

            public GetWarehouseByProductIdAndSizeQueryHandler(IWarehouseRepository wareHouseRepository, IMediator mediator)
            {
                _wareHouseRepository = wareHouseRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<Warehouse>> Handle(GetWarehouseByProductIdAndSizeQuery request, CancellationToken cancellationToken)
            {

                var wareHouseByProductIdAndSize = await _wareHouseRepository.GetAsync(x => x.ProductId == request.ProductId && x.Status == true && x.isDeleted == false);
                return new SuccessDataResult<Warehouse>(wareHouseByProductIdAndSize);
            }
        }
    }
}
