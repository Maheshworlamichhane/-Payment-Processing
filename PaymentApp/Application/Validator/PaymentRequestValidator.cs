using Domain.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class PaymentRequestValidator : AbstractValidator<PaymentRequestDto>
    {
        public PaymentRequestValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required.")
                .Length(3).WithMessage("Currency must be a 3-letter code.");

            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Customer name is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Customer email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("Payment method is required.")
                .Must(method => method == "Card" || method == "BankTransfer")
                .WithMessage("Payment method must be either 'Card' or 'BankTransfer'.");

            RuleFor(x => x.CardOrAccountNumber)
                .NotEmpty().WithMessage("Card or Account number is required.")
                .Length(8, 16).WithMessage("Card/Account number must be between 8 and 16 characters.");
        }
    }
}
