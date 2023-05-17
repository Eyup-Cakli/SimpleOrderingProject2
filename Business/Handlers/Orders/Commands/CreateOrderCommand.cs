
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
using Business.Handlers.Orders.ValidationRules;
using Business.Handlers.Warehouses.Commands;
using System;
using Business.Handlers.Warehouses.Queries;

namespace Business.Handlers.Orders.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateOrderCommand : IRequest<IResult>
    {

        public int CreatedUserId { get; set; }
        public int LastUpdatedUserId { get; set; }
        public bool Status { get; set; }
        public bool isDeleted { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }


        public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, IResult>
        {
            private readonly IOrderRepository _orderRepository;
            private readonly IMediator _mediator;
            public CreateOrderCommandHandler(IOrderRepository orderRepository, IMediator mediator)
            {
                _orderRepository = orderRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateOrderValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
            {
                var isThereOrderRecord = _orderRepository.Query().Any(u => u.ProductId==request.ProductId && u.CustomerId==request.CustomerId && u.Quantity==request.Quantity && u.isDeleted == false );

                if (isThereOrderRecord == true)
                    return new ErrorResult(Messages.NameAlreadyExist);

                var wareHouseRecord = await _mediator.Send(new GetWarehouseByProductIdAndSizeQuery { ProductId = request.ProductId});//size

                wareHouseRecord.Data.Quantity = wareHouseRecord.Data.Quantity - request.Quantity;

                var updatedWareHouse = await _mediator.Send(new UpdateWarehouseCommand
                {
                    Quantity = wareHouseRecord.Data.Quantity,
                    //CreatedDate = wareHouseRecord.Data.CreatedDate,
                    CreatedUserId = wareHouseRecord.Data.CreatedUserId,
                    Id = wareHouseRecord.Data.Id,
                    isDeleted = wareHouseRecord.Data.isDeleted,
                    Status = wareHouseRecord.Data.Quantity != 0,
                    ReadyForSale = wareHouseRecord.Data.ReadyForSale,
                   // LastUpdatedDate = wareHouseRecord.Data.LastUpdatedDate,
                    LastUpdatedUserId = wareHouseRecord.Data.LastUpdatedUserId,
                    ProductId = wareHouseRecord.Data.ProductId,
                    //Size = wareHouseRecord.Data.Size,
                });

                if (updatedWareHouse.Success)
                {
                    var addedOrder = new Order
                    {
                        CreatedUserId = request.CreatedUserId,
                        CreatedDate = DateTime.Now,
                        LastUpdatedUserId = request.LastUpdatedUserId,
                        LastUpdatedDate = DateTime.Now,
                        Status = request.Status,
                        isDeleted = request.isDeleted,
                        CustomerId = request.CustomerId,
                        ProductId = request.ProductId,
                        Quantity = request.Quantity,
                        //Size = request.Size,

                        //depo var mı yokmu , varsa depoyu getir. 
                        //depodan siparişteki adet sayısını düş
                        //ardından depodan adet sayısını güncelle.
                        //order kaydet
                        //depo yoksa uyaru bas.
                    };
                    _orderRepository.Add(addedOrder);
                    await _orderRepository.SaveChangesAsync();
                    return new SuccessResult(Messages.Added);
                }
                return new ErrorResult(updatedWareHouse.Message);
            }
        }
    }
}