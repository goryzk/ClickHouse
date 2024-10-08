﻿using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace ClickHousePubisher;

public class PushAsJson
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
        var exchangeName = "exchange_asString";
        var routingKey = "queue_AsJson";
        var queue = "clickhouse_asJson";

        channel.ExchangeDeclare(exchange: exchangeName, type: "direct", durable: true);

        channel.QueueDeclare(queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.QueueBind(queue: queue, exchange: exchangeName, routingKey: routingKey);

        for (int i = 0; i < 10000000; i++)
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
            {
                key = Random.Shared.Next(0, 1800),
                value = Random.Shared.Next(0, 1800),
            }));

            channel.BasicPublish(exchange: exchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: body);
        }

        Console.WriteLine(" [x] Sent message with routing key '{0}'", routingKey);
    }
}