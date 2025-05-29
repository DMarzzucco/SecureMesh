
namespace Security.Queues.Interfaces;

/// <summary>
/// Rabbit Interface
/// </summary>
public interface IRabbitMQServices { Task SendMessageAsync<T>(T message, string queueName); }
