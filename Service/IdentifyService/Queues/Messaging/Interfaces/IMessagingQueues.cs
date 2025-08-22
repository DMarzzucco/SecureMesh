using System;

namespace IdentifyService.Queues.Messaging.Interfaces;

public interface IMessagingQueues
{
    Task SendWelcomeMessage(string fullName, string email, int id);
    Task SendNewEmailVerificationEvent(string email, string token, int id);
    Task SendEmailVerificactionEvent(string email, string token, int id);
    Task PasswordRecuperationMessage(string email, string token, int id);
    Task TowAfCodeMessage(string email, string code);
    Task RiskBasedAuthenticationMessage(string token, string email, string userAgent, string location);
}
