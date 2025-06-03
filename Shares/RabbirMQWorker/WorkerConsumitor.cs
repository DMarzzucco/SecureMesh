using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitWorkerCore;

public interface IMessageHandler<T> { Task HandleAsync(T message); }

public class WorkerConsumitor<T>
{
    private readonly string _queueNames;
    private readonly IMessageHandler<T> _handler;
    private readonly ConnectionFactory _factory;

    public WorkerConsumitor(string queueNames, IMessageHandler<T> handler)
    {
        _queueNames = queueNames;
        _handler = handler;

        _factory = new ConnectionFactory
        {
            // HostName = "localhost",
            HostName = "rabbitmq",
            UserName = "user",
            Password = "password",
            Port = 5672
        };
    }

    public async Task StartAsync()
    {
        using var connection = await this._factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: this._queueNames,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);
            var message = JsonSerializer.Deserialize<T>(json);
            if (message is not null)
            {
                await this._handler.HandleAsync(message);
                await channel.BasicAckAsync(ea.DeliveryTag, false);
            }
        };

        await channel.BasicConsumeAsync(queue: this._queueNames, autoAck: false, consumer: consumer);

        Console.WriteLine($"Escuchando la cola '{this._queueNames}. Presino [Enter] para salir...");
        // Console.ReadLine();
        await Task.Delay(Timeout.Infinite);
    }
}
