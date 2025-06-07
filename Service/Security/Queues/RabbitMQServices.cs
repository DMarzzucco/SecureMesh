using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Security.Queues.Interfaces;

namespace Security.Queues;

/// <summary>
/// RabbitMQ Service
/// </summary>
public class RabbitMQServices : IRabbitMQServices
{
    private readonly ConnectionFactory _factory;

    public RabbitMQServices()
    {
        _factory = new ConnectionFactory
        { 
            // HostName = "localhost",
            HostName = "rabbitmq",
            UserName = "user",
            Password = "password",
            Port = 5672
        };
    }

    /// <summary>
    /// SendMessageAsync
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendMessageAsync<T>(T message, string queueName)
    {
        using var connection = await this._factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();



        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            mandatory: true,
            basicProperties: new BasicProperties { Persistent = true },
            body: body);
    }
}
