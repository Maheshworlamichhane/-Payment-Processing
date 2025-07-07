//using System.Text.Json;
//using System.Threading.Tasks;
//using Application.Common.Events;
//using Confluent.Kafka;

//namespace Infrastructure.Messaging
//{
//    public class KafkaPublisher : IPublisher
//    {
//        private readonly IProducer<string, string> _producer;
//        private const string TopicName = "payments";

//        public KafkaPublisher(ProducerConfig config)
//        {
//            var producerBuilder = new ProducerBuilder<string, string>(config);
//            _producer = producerBuilder.Build();
//        }

//        public async Task PublishAsync<T>(T @event) where T : class
//        {
//            // Serialize event object to JSON
//            var messageValue = JsonSerializer.Serialize(@event);

//            var message = new Message<string, string>
//            {
//                Key = Guid.NewGuid().ToString(), // Unique key for partitioning
//                Value = messageValue
//            };

//            try
//            {
//                // Send to Kafka topic
//                var deliveryResult = await _producer.ProduceAsync(TopicName, message);

//                Console.WriteLine($"✅ Event published to {TopicName} at offset {deliveryResult.Offset}");
//            }
//            catch (ProduceException<string, string> ex)
//            {
//                Console.WriteLine($"❌ Failed to publish event: {ex.Error.Reason}");
//                throw;
//            }
//        }
//    }
//}

using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Common.Events;
using Confluent.Kafka;

namespace Infrastructure.Messaging
{
    public class KafkaPublisher : IPublisher
    {
        private readonly IProducer<string, string> _producer;
        private const string TopicName = "payments";

        public KafkaPublisher(ProducerConfig config)
        {
            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task PublishAsync<T>(T @event) where T : class
        {
            var messageValue = JsonSerializer.Serialize(@event);

            var message = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(), 
                Value = messageValue,
                Headers = new Headers
                {
                    { "eventType", Encoding.UTF8.GetBytes(typeof(T).Name) } 
                }
            };

            try
            {
                var result = await _producer.ProduceAsync(TopicName, message);
                Console.WriteLine($"✅ Event [{typeof(T).Name}] published to {TopicName} at offset {result.Offset}");
            }
            catch (ProduceException<string, string> ex)
            {
                Console.WriteLine($"❌ Failed to publish {typeof(T).Name}: {ex.Error.Reason}");
                throw;
            }
        }
    }
}

