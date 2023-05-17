using Amazon.Runtime.Internal;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Customers.Queries
{
    public class GetCustomerLookUpQuery : IRequest<IDataResult<IEnumerable<SelectionItem>>>
    {
        public class GetCustomerLookUpQueryHandler : IRequestHandler<GetCustomerLookUpQuery, IDataResult<IEnumerable<SelectionItem>>>
        {
            private readonly ICustomerRepository _customerRepository;
            private readonly IMediator _mediator;

            public GetCustomerLookUpQueryHandler(ICustomerRepository customerRepository, IMediator mediator)
            {
                _customerRepository = customerRepository;
                _mediator = mediator;
            }

            [SecuredOperation(Priority = 1)]
            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            public async Task<IDataResult<IEnumerable<SelectionItem>>> Handle(GetCustomerLookUpQuery request, CancellationToken cancellationToken)
            {
                return new SuccessDataResult<IEnumerable<SelectionItem>>(
                    await _customerRepository.GetCustomerLookUp());
            }
        }
    }
}
