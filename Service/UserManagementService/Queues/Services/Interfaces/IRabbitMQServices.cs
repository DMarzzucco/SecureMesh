using System;

namespace UserManagementService.Queues.Services.Interfaces;

public interface IRabbitMQServices { Task SendMessageAsync<T>(T message, string queueName); }
