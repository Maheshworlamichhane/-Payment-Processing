// Domain/Events/PaymentInitiatedEvent.cs
namespace Domain.Events;

public class PaymentInitiatedEvent
{
    public Guid TransactionId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
}


public class PaymentCompletedEvent
{
    public Guid TransactionId { get; set; }
    public DateTime CompletedAt { get; set; }
}

public class PaymentFailedEvent
{
    public Guid TransactionId { get; set; }
    public string Reason { get; set; }
    public DateTime FailedAt { get; set; }
}
