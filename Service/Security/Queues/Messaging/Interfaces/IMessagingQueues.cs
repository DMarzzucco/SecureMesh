using System;

namespace Security.Queues.Messaging.Interfaces;

public interface IMessagingQueues
{
    Task SendWelcomeMessage(string fullName, string email, int id);
    Task SendNewEmailVerificationEvent(string email, string token, int id);
    Task SendEmailVerificactionEvent(string email, string token, int id);
    Task PasswordRecuperationMessage(string email, string token, int id);
}
