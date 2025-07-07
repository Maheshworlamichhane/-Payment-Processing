using MediatR;

public class GetTransactionsQuery : IRequest<IEnumerable<TransactionDto>>
{
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Status { get; init; }
    public string? UserId { get; init; }
}
