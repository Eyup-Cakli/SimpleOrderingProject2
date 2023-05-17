using Amazon.Runtime.Internal;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Warehouses.Queries
{
    public class GetSynchronOrderQuery : IRequest<IDataResult<bool>>
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string ReadyForSale { get; set; }
        public bool isDeleted { get; set; }

        public class GetSynchronOrderQueryHandler : IRequestHandler<GetSynchronOrderQuery, IDataResult<bool>>
        {
            private readonly IWarehouseRepository _warehouseRepository;
            private readonly IMediator _mediator;

            public GetSynchronOrderQueryHandler(IWarehouseRepository warehouseRepository, IMediator mediator)
            {
                _warehouseRepository = warehouseRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<bool>> Handle(GetSynchronOrderQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<bool>(await _warehouseRepository.SynchronOrder(request.ProductId, request.Quantity, request.ReadyForSale, request.isDeleted));
            }
        }
    }
}
