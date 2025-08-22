using System;
using UserManagementService.Queues.Infrastructure;
using UserManagementService.Queues.Messaging.Helper;
using UserManagementService.Queues.Messaging.Interfaces;
using UserManagementService.Queues.Services.Interfaces;

namespace UserManagementService.Queues.Messaging;

public class MessagingQueues(IRabbitMQServices serivces) : IMessagingQueues
{
    private readonly IRabbitMQServices _serivces = serivces;

    /// <summary>
    /// Password Recuperation Message
    /// </summary>
    /// <param name="email"></param>
    /// <param name="token"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task PasswordRecuperationMessage(string email, string token, int id)
    {
        var message = new RecuperationAccountMessage { Email = email, Token = token, Id = id };
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        await this._serivces.SendMessageAsync(message, QueuesNames.PasswordRecuperationQeue);

    }
}
