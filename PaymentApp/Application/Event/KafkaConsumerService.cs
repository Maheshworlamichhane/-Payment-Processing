

//using Confluent.Kafka;
//using Infrastructure;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using System.Text.Json;


//public class PaymentConsumerService : BackgroundService
//{
//    private readonly ILogger<PaymentConsumerService> _logger;
//    private readonly IServiceScopeFactory _scopeFactory;
//    private readonly IConsumer<string, string> _consumer;
//    private readonly Random _random = new();
//    private static readonly string[] Statuses = { "SUCCESS", "PENDING", "FAILED" };

//    public PaymentConsumerService(ILogger<PaymentConsumerService> logger, IServiceScopeFactory scopeFactory)
//    {
//        _logger = logger;
//        _scopeFactory = scopeFactory;

//        var config = new ConsumerConfig
//        {
//            BootstrapServers = "localhost:9092",
//            GroupId = "payments",
//            AutoOffsetReset = AutoOffsetReset.Earliest
//        };

//        _consumer = new ConsumerBuilder<string, string>(config).Build();
//        _consumer.Subscribe("payments");
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        _logger.LogInformation("Kafka Consumer started...");

//        while (!stoppingToken.IsCancellationRequested)
//        {
//            try
//            {
//                var result = _consumer.Consume(TimeSpan.FromSeconds(1));

//                if (result == null)
//                {
//                    await Task.Delay(1000, stoppingToken); // Poll again after short delay
//                    continue;
//                }

//                var messageValue = result.Message.Value;
//                _logger.LogInformation("Received event: {Message}", messageValue);

//                await Task.Delay(3000, stoppingToken); // Simulate processing delay

//                using var scope = _scopeFactory.CreateScope();
//                var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();

//                var paymentEvent = JsonSerializer.Deserialize<PaymentEvent>(messageValue, new JsonSerializerOptions
//                {
//                    PropertyNameCaseInsensitive = true
//                });

//                if (paymentEvent == null || paymentEvent.PaymentId == Guid.Empty)
//                {
//                    _logger.LogWarning("Failed to deserialize or missing PaymentId: {Message}", messageValue);
//                    continue;
//                }

//                var transaction = await dbContext.Transactions
//                    .FirstOrDefaultAsync(t => t.TransactionID == paymentEvent.PaymentId, stoppingToken);

//                if (transaction != null)
//                {
//                    var randomStatus = Statuses[_random.Next(Statuses.Length)];
//                    transaction.Status = randomStatus;

//                    dbContext.Update(transaction);
//                    await dbContext.SaveChangesAsync(stoppingToken);

//                    _logger.LogInformation("Updated transaction {Id} to status {Status}", paymentEvent.PaymentId, randomStatus);
//                }
//                else
//                {
//                    _logger.LogWarning("Transaction with ID {Id} not found", paymentEvent.PaymentId);
//                }
//            }
//            catch (ConsumeException ex)
//            {
//                _logger.LogError(ex, "Kafka consume error");
//            }
//            catch (OperationCanceledException)
//            {
//                _logger.LogInformation("Kafka consumer cancellation requested.");
//                break;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Unexpected error in Kafka consumer");
//            }
//        }
//    }

//    public override void Dispose()
//    {
//        _consumer.Close();
//        _consumer.Dispose();
//        base.Dispose();
//    }

//    public class PaymentEvent
//    {
//        public Guid PaymentId { get; set; }
//        public decimal Amount { get; set; }
//    }
//}



using Confluent.Kafka;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class PaymentConsumerService : BackgroundService
{
    private readonly ILogger<PaymentConsumerService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConsumer<string, string> _consumer;
    private readonly Random _random = new();
    private static readonly string[] Statuses = { "success", "pending", "failed" };

    public PaymentConsumerService(ILogger<PaymentConsumerService> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;

        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "payments",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _consumer.Subscribe("payments");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka Consumer started...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(TimeSpan.FromSeconds(1));
                if (result == null)
                {
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                var messageValue = result.Message.Value;
                _logger.LogInformation("Received event: {Message}", messageValue);

                await Task.Delay(3000, stoppingToken); // Simulate processing

                using var scope = _scopeFactory.CreateScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<EmailService>(); // ✅ Resolved here

                var paymentEvent = JsonSerializer.Deserialize<PaymentEvent>(messageValue, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (paymentEvent == null || paymentEvent.PaymentId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid payment event or missing PaymentId: {Message}", messageValue);
                    continue;
                }

                var transaction = await dbContext.Transactions
                    .FirstOrDefaultAsync(t => t.TransactionID == paymentEvent.PaymentId, stoppingToken);

                if (transaction != null)
                {
                    var randomStatus = Statuses[_random.Next(Statuses.Length)];
                    transaction.Status = randomStatus;

                    dbContext.Update(transaction);
                    await dbContext.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation("Updated transaction {Id} to status {Status}", paymentEvent.PaymentId, randomStatus);

                    if (randomStatus == "success".ToLower())
                    {
                        await emailService.SendEmailAsync(paymentEvent.Email,
                            //"maheshworlamichhane4@gmail.com", // You can make this dynamic later
                            "✅ Payment Successful",
                            $"🎉 Your payment of Rs. {paymentEvent.Amount} with ID {paymentEvent.PaymentId} was successful."
                        );

                        _logger.LogInformation("Email notification sent.");
                    }
                    else if (randomStatus == "failed".ToLower())
                    {
                        await emailService.SendEmailAsync(paymentEvent.Email,
                           //"maheshworlamichhane4@gmail.com", // You can make this dynamic later
                           "✅ Payment Failed",
                           $"🎉 Your payment of Rs. {paymentEvent.Amount} with ID {paymentEvent.PaymentId} was Failed."
                       );

                        _logger.LogInformation("Email notification sent.");
                    }
                    else if (randomStatus =="pending".ToLower())
                    {
                        await emailService.SendEmailAsync(paymentEvent.Email,
                           //"maheshworlamichhane4@gmail.com", // You can make this dynamic later
                           "✅ Payment Pending",
                           $"🎉 Your payment of Rs. {paymentEvent.Amount} with ID {paymentEvent.PaymentId} was Pending."
                       );

                        _logger.LogInformation("Email notification sent.");
                    }
                }
                else
                {
                    _logger.LogWarning("Transaction not found: ID {Id}", paymentEvent.PaymentId);
                }
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Kafka consume error");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Kafka consumer stopped.");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in consumer");
            }
        }
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }

    public class PaymentEvent
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Email { get; set; }
    }
}
