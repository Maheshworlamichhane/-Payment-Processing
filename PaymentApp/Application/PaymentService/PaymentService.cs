using Application.Common.Events;
using Application.IPaymentService;
using Domain.DTOs;
using Domain.Models;
using FluentValidation;
using Infrastructure;
using System.Globalization;

public class PaymentService : IPayment
{
    private readonly IPublisher _publisher;
    private readonly PaymentDbContext _context;
    private readonly IValidator<PaymentRequestDto> _validator;

    public PaymentService(
        IPublisher publisher,
        PaymentDbContext context,
        IValidator<PaymentRequestDto> validator)
    {
        _publisher = publisher;
        _context = context;
        _validator = validator;
    }

    public async Task<PaymentResultDto> ProcessPaymentAsync(PaymentRequestDto request, string userid)
    {
        // Validate the incoming request DTO
        await _validator.ValidateAndThrowAsync(request);

        // Create transaction entity
        var payment = new Transaction
        {
            TransactionID = Guid.NewGuid(),
            Amount = request.Amount,
            Currency = request.Currency,
            Status = "Initiated",
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            UserId = userid,


        };

        _context.Transactions.Add(payment);
        await _context.SaveChangesAsync();

        // Create and publish PaymentInitiatedEvent
        var paymentInitiated = new PaymentInitiatedEvent
        {
            PaymentId = payment.TransactionID,
            Amount = request.Amount,
            Email = request.Email
        };

        await _publisher.PublishAsync(paymentInitiated);
        // Return result to the caller
        return new PaymentResultDto
        {
            Success = true,
            Status = "Initiated",
            TransactionId = payment.TransactionID.ToString(),
            Message = "Payment request initiated successfully."
        };
    }
}
