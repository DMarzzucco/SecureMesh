using System;

namespace UserManagementService.Queues.Messaging.Interfaces;

public interface IMessagingQueues
{
    Task PasswordRecuperationMessage(string email, string token, int id);

}
