using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, IEnumerable<TransactionDto>>
{
    private readonly IConfiguration _configuration;

    public GetTransactionsQueryHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IEnumerable<TransactionDto>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        

        var parameters = new DynamicParameters();
        parameters.Add("@Status", request.Status);
        parameters.Add("@UserId", request.UserId);

        var result = await connection.QueryAsync<TransactionDto>(
            "GetFilteredTransactions",
            parameters,
            commandType: CommandType.StoredProcedure
        );
        if(cancellationToken.IsCancellationRequested)
        {
            return Enumerable.Empty<TransactionDto>();
        }
        return result;
    }

}

