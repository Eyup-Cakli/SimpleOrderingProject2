﻿
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Business.Handlers.Customers.ValidationRules;

namespace Business.Handlers.Customers.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateCustomerCommand : IRequest<IResult>
    {

        public int CreatedUserId { get; set; }
        public int LastUpdatedUserId { get; set; }
        public bool Status { get; set; }
        public bool isDeleted { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerCode { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }


        public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, IResult>
        {
            private readonly ICustomerRepository _customerRepository;
            private readonly IMediator _mediator;
            public CreateCustomerCommandHandler(ICustomerRepository customerRepository, IMediator mediator)
            {
                _customerRepository = customerRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateCustomerValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
            {
                var isThereCustomerRecord = _customerRepository.Query().Any(u => u.PhoneNumber == request.PhoneNumber && u.FirstName == request.FirstName && u.LastName == request.LastName && u.Email == request.Email && u.CustomerCode == request.CustomerCode && u.Address == request.Address && u.isDeleted == false);

                if (isThereCustomerRecord == true) 
                {
                    return new ErrorResult(Messages.NameAlreadyExist);
                }
                    

                else
                {
                    var addedCustomer = new Customer
                    {
                        CreatedUserId = request.CreatedUserId,
                        CreatedDate = System.DateTime.Now,
                        Status = request.Status,
                        isDeleted = false,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        CustomerCode = request.CustomerCode,
                        Address = request.Address,
                        PhoneNumber = request.PhoneNumber,
                        Email = request.Email,

                    };

                    _customerRepository.Add(addedCustomer);
                    await _customerRepository.SaveChangesAsync();
                    return new SuccessResult(Messages.Added);
                }
            }
        }
    }
}