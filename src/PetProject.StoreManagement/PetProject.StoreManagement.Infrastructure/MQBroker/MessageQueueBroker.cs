using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using PetProject.StoreManagement.Domain.ThirdPartyServices.MQBroker;

namespace PetProject.StoreManagement.Infrastructure.MQBroker
{
    public class MessageQueueBroker : IMessageQueueBroker
    {
        private readonly IConnectionFactory _connectionFactory;

        public MessageQueueBroker()
        {
            // Note: RabbitMQ auto defined PORT
            _connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost",
            };
        }

        public void SendMessage<T>(T data)
        {
            var connection = _connectionFactory.CreateConnection();
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(nameof(T), exclusive: false);
                var jsonData = JsonSerializer.Serialize(data);
                var body = Encoding.UTF8.GetBytes(jsonData);
                channel.BasicPublish(exchange: "", routingKey: nameof(T), body: body);
            }
        }

        public void SendMessage<T>(T data, string queueName)
        {
            var connection = _connectionFactory.CreateConnection();
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queueName, exclusive: false);
                var jsonData = JsonSerializer.Serialize(data);
                var body = Encoding.UTF8.GetBytes(jsonData);
                channel.BasicPublish(exchange: "", routingKey: queueName, body: body);
            }
        }

        public void ReceiveMessage(string queueName, Action<object?, string> action)
        {
            var connection = _connectionFactory.CreateConnection();
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queueName);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, eventArgs) =>
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    action(sender, message);
                };
            }
        }
    }
}