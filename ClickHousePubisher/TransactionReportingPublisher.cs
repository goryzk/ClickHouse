using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bogus;
using RabbitMQ.Client;

namespace ClickHousePubisher;

public class TransactionReportingPublisher
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
            .RuleFor(t => t.Id, f => f.Random.Int(1, 9999999))
            .RuleFor(t => t.PaymentSystemTransactionId, f => f.Random.Guid().ToString())
            .RuleFor(t => t.MerchantTransactionId, f => f.Random.Guid().ToString())
            .RuleFor(t => t.OperationType, f => f.PickRandom(new[] { 1, 2 }))
            .RuleFor(t => t.State, f => f.PickRandom(new[] { 1, 2, 3, 4 }))
            .RuleFor(t => t.Amount, f => f.Finance.Amount())
            .RuleFor(t => t.Currency, f => 851)
            .RuleFor(t => t.PaymentMethodId, f => f.Random.Int(1, 2))
            .RuleFor(t => t.SiteId, f => f.Random.Int(1, 1800))
            .RuleFor(t => t.PaymentSystemId, f => f.Random.Int(1, 9999999))
            .RuleFor(t => t.InitAmount, f => f.Finance.Amount());

        for (var i = 0; i < 10000; i++)
        {
            var transaction = faker.Generate();
            var json = JsonSerializer.Serialize(new
            {
                id = transaction.Id,
                paymentsystemtransactionid = transaction.PaymentSystemTransactionId,
                merchanttransactionid = transaction.MerchantTransactionId,
                creationdate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                lastupdatedate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                operationtype = transaction.OperationType,
                state = transaction.State,
                amount = transaction.Amount,
                currency = transaction.Currency,
                paymentmethodid = transaction.PaymentMethodId,
                siteid = transaction.SiteId,
                paymentsystemid = transaction.PaymentSystemId,
                initamount = transaction.InitAmount,
            });
            
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: exchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: body);
        }

        Console.WriteLine(" [x] Sent message with routing key '{0}'", routingKey);
    }
}

public class Transaction
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("paymentsystemtransactionid")]
    public string PaymentSystemTransactionId { get; set; }

    [JsonPropertyName("merchanttransactionid")]
    public string MerchantTransactionId { get; set; }

    [JsonPropertyName("creationdate")]
    public DateTime CreationDate { get; set; }

    [JsonPropertyName("lastupdatedate")]
    public DateTime LastUpdateDate { get; set; }

    [JsonPropertyName("operationtype")]
    public int OperationType { get; set; }

    [JsonPropertyName("state")]
    public int State { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public int Currency { get; set; }

    [JsonPropertyName("paymentmethodid")]
    public int PaymentMethodId { get; set; }

    [JsonPropertyName("siteid")]
    public int SiteId { get; set; }

    [JsonPropertyName("paymentsystemid")]
    public int PaymentSystemId { get; set; }

    [JsonPropertyName("initamount")]
    public decimal InitAmount { get; set; }
}
