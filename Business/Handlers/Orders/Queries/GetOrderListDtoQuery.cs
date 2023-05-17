using Amazon.Runtime.Internal;
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
    public class GetOrderListDtoQuery: IRequest<IDataResult<IEnumerable<OrderDto>>>
    {
        public class
            GetOrderListQueryHandler : IRequestHandler<GetOrderListDtoQuery,
                IDataResult<IEnumerable<OrderDto>>>
        {
            public readonly IOrderRepository _orderRepository;
            public readonly IMediator _mediator;

            public GetOrderListQueryHandler(IOrderRepository orderRepository, IMediator mediator)
            {
                _orderRepository = orderRepository;
                _mediator = mediator;
            }

            public async Task<IDataResult<IEnumerable<OrderDto>>> Handle(GetOrderListDtoQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<OrderDto>>(await _orderRepository.GetOrderDto());
            }
        }
    }
}
