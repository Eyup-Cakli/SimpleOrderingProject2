
using Business.Handlers.Customers.Commands;
using FluentValidation;

namespace Business.Handlers.Customers.ValidationRules
{

    public class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerValidator()
        {
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.CustomerCode).NotEmpty();
            RuleFor(x => x.Address).NotEmpty();
            RuleFor(x => x.PhoneNumber).NotEmpty();
            RuleFor(x => x.Email).NotEmpty();

        }
    }
    public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerValidator()
        {
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.CustomerCode).NotEmpty();
            RuleFor(x => x.Address).NotEmpty();
            RuleFor(x => x.PhoneNumber).NotEmpty();
            RuleFor(x => x.Email).NotEmpty();

        }
    }
}