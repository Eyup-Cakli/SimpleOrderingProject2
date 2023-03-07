
using Business.Handlers.Warehouses.Commands;
using FluentValidation;

namespace Business.Handlers.Warehouses.ValidationRules
{

    public class CreateWarehouseValidator : AbstractValidator<CreateWarehouseCommand>
    {
        public CreateWarehouseValidator()
        {
            RuleFor(x => x.Quantity).NotEmpty();
            RuleFor(x => x.ReadyForSale).NotEmpty();

        }
    }
    public class UpdateWarehouseValidator : AbstractValidator<UpdateWarehouseCommand>
    {
        public UpdateWarehouseValidator()
        {
            RuleFor(x => x.Quantity).NotEmpty();
            RuleFor(x => x.ReadyForSale).NotEmpty();

        }
    }
}