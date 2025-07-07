public class TransactionDto
{
    public Guid TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public string Currency { get; set; }
    public string Timestamp { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; }
}
