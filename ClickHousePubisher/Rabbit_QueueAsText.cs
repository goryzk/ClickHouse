using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace ClickHousePubisher;

public class Rabbit_QueueAsText
{
    public void Push()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "username",
            Password = "password",
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        var exchangeName = "exchange";
        var routingKey = "format=logs,type=report,year=2020";
        var queue = "clickhouse";

        channel.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: true);

        channel.QueueDeclare(queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.QueueBind(queue: queue, exchange: exchangeName, routingKey: routingKey);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
        {
            key = 9,
            value = "11new value 43"
        }));

        channel.BasicPublish(exchange: exchangeName,
            routingKey: routingKey,
            basicProperties: null,
            body: body);

        Console.WriteLine(" [x] Sent message with routing key '{0}'", routingKey);
    }
}