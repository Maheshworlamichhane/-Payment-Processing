using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
[Authorize]
[ApiController]
[Route("transactions")]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAllTransactions(
        [FromQuery] string? status,
        [FromQuery] string? userId)
    {
        var query = new GetTransactionsQuery
        {
            Status = status,
            UserId = userId
        };
     
        var transactions = await _mediator.Send(query);

        return Ok(transactions);
    }
}

