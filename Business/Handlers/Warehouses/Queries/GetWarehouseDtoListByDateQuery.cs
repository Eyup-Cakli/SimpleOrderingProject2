using Amazon.Runtime.Internal;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
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
    public class GetWarehouseDtoListByDateQuery : IRequest<IDataResult<IEnumerable<WarehouseDto>>>
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public class
            GetWarehouseDtoListByDateQueryHandler : IRequestHandler<GetWarehouseDtoListByDateQuery,
                IDataResult<IEnumerable<WarehouseDto>>>
        {
            public readonly IWarehouseRepository _warehouseRepository;
            public readonly IMediator _mediator;

            public GetWarehouseDtoListByDateQueryHandler(IWarehouseRepository warehouseRepository, IMediator mediator)
            {
                _warehouseRepository = warehouseRepository;
                _mediator = mediator;
            }

            [SecuredOperation(Priority = 1)]
            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            public async Task<IDataResult<IEnumerable<WarehouseDto>>> Handle(GetWarehouseDtoListByDateQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<WarehouseDto>>(await _warehouseRepository.GetWarehouseDtoByDate(request.StartDate, request.EndDate));
            }
        }
    }
}
