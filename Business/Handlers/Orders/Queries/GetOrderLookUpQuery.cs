using System;
using System.Collections.Generic;
using System.Linq;
using Core.Aspects.Autofac.Caching;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using Core.Utilities.Results;
using Core.Entities.Dtos;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Aspects.Autofac.Logging;

namespace Business.Handlers.Orders.Queries
{
    public class GetOrderLookUpQuery:IRequest<IDataResult<IEnumerable<SelectionItem>>>
    {
        public class GetOrderLookUpQueryHandler: IRequestHandler<GetOrderLookUpQuery, IDataResult<IEnumerable<SelectionItem>>>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IMediator _mediator;

            public GetOrderLookUpQueryHandler(IOrderRepository orderRepository, IMediator mediator)
            {
                _orderRepository = orderRepository;
                _mediator = mediator;
            }

            [SecuredOperation(Priority = 1)]
            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            public async Task<IDataResult<IEnumerable<SelectionItem>>>Handle(GetOrderLookUpQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<SelectionItem>>(
                    await _orderRepository.GetOrderLookUp());
            }
        }

    }
}
