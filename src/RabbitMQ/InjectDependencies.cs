using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System.Text;

namespace RabbitMQ;

public static class InjectDependencies
{
    public static void AddRabbitMq(this IServiceCollection services) 
    {
        ConnectionFactory rabbitFactory = new ConnectionFactory();

        var connection = rabbitFactory.CreateConnection();

        using var channel = connection.CreateModel();
        channel.QueueDeclare(
            "Testing",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
            );

        string queueMessage = $"This was sent as a test at: {DateTime.UtcNow}";

        channel.BasicPublish(exchange: "", routingKey: "Testing", basicProperties: null, body: Encoding.UTF8.GetBytes(queueMessage));

        connection.Close();
    }
}
