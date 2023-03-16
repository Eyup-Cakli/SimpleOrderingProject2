using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Warehouses.Queries
{
    public class GetWarehouseLookupQuery : IRequest<IDataResult<IEnumerable<SelectionItem>>>
    {
        public class GetWarehouseLookupQueryHandler : IRequestHandler<GetWarehouseLookupQuery,
                IDataResult<IEnumerable<SelectionItem>>>
        {
            private readonly IWarehouseRepository _warehouseRepository;
            private readonly IMediator _mediator;

            public GetWarehouseLookupQueryHandler(IWarehouseRepository warehouseRepository, IMediator mediator)
            {
                _warehouseRepository = warehouseRepository;
                _mediator = mediator;
            }

            [SecuredOperation(Priority = 1)]
            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            public async Task<IDataResult<IEnumerable<SelectionItem>>> Handle(GetWarehouseLookupQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<SelectionItem>>(
                    await _warehouseRepository.GetWarehouseLookUp());
            }
        }
    }
}
