using System.Text;
using System.Text.Json;
using Bogus;
using RabbitMQ.Client;

namespace ClickHouseReporting;

public class TransactionReporting
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
        
        var exchangeName = "exchange_reporting";
        var routingKey = "reporting";
        var queue = "reporting";

        channel.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: true);

        channel.QueueDeclare(queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.QueueBind(queue: queue, exchange: exchangeName, routingKey: routingKey);

        var faker = new Faker<Transaction>()
            .RuleFor(t => t.PaymentSystemTransactionId, f => f.Random.Guid().ToString())
            .RuleFor(t => t.MerchantTransactionId, f => f.Random.Guid().ToString())
            .RuleFor(t => t.OperationType, f => f.PickRandom(new[] { 1, 2 }))
            .RuleFor(t => t.State, f => f.PickRandom(new[] { 1, 2, 3, 4 }))
            .RuleFor(t => t.Amount, f => f.Finance.Amount())
            .RuleFor(t => t.Currency, f => 851)
            .RuleFor(t => t.PaymentMethod, f => f.Random.Int(1, 2))
            .RuleFor(t => t.SiteId, f => f.Random.Int(1, 1800))
            .RuleFor(t => t.PaymentSystemId, f => f.Random.Int(1, 9999999))
            .RuleFor(t => t.InitAmount, f => f.Finance.Amount())
            .RuleFor(t => t.Language, f => "en")
            .RuleFor(t => t.IsBlockedTransaction, f => false)
            .RuleFor(t => t.CreationDate, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            .RuleFor(t => t.LastUpdateDate, f => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        for (var i = 0; i < 10000; i++)
        {
            var transaction = faker.Generate();
            var json = JsonSerializer.Serialize(transaction);
            
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: exchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: body);
        }

        Console.WriteLine(" [x] Sent message with routing key '{0}'", routingKey);
    }
}