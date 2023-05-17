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

namespace Business.Handlers.Orders.Queries
{
    public class GetOrderDtoListByDateQuery : IRequest<IDataResult<IEnumerable<OrderDto>>>
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public class GetOrderDtoListByDateQueryHandler : 
            IRequestHandler<GetOrderDtoListByDateQuery, IDataResult<IEnumerable<OrderDto>>>
        {
            public readonly IOrderRepository _orderRepository;
            public readonly IMediator _mediator;

            public GetOrderDtoListByDateQueryHandler(IOrderRepository orderRepository, IMediator mediator)
            {
                _orderRepository = orderRepository;
                _mediator = mediator;
            }

            [SecuredOperation(Priority = 1)]
            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            public async Task<IDataResult<IEnumerable<OrderDto>>> Handle(GetOrderDtoListByDateQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<OrderDto>>(await _orderRepository.getOrderListDtoByDate(request.StartDate, request.EndDate));
            }
        }
    }
}
