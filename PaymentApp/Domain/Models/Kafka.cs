public class PaymentInitiatedEvent
{
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Email { get; set; }
}