using Security.Queues.Infrastructure;
using Security.Queues.Interfaces;
using Security.Queues.Messaging.Interfaces;

namespace Security.Queues.Messaging;

public partial class MessagingQueues(IRabbitMQServices services) : IMessagingQueues
{
    private readonly IRabbitMQServices _services = services;

    /// <summary>
    /// Send Email Verification
    /// </summary>
    /// <param name="email"></param>
    /// <param name="token"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task SendEmailVerificactionEvent(string email, string token, int id)
    {
        var message = new EmailVerifcationMessage
        {
            Email = email,
            Token = token,
            Id = id,
        };
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        await _services.SendMessageAsync(message, QueuesNames.EmailVerficationQueue);
    }
    /// <summary>
    /// Verificate New Email
    /// </summary>
    /// <param name="email"></param>
    /// <param name="token"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task SendNewEmailVerificationEvent(string email, string token, int id)
    {
        var message = new EmailVerifcationMessage
        {
            Email = email,
            Token = token,
            Id = id,
        };
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        await _services.SendMessageAsync(message, QueuesNames.NewEmailVerificationQueue);
    }
    /// <summary>
    /// Send Welcome Message
    /// </summary>
    /// <param name="fullName"></param>
    /// <param name="email"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task SendWelcomeMessage(string fullName, string email, int id)
    {
        var message = new WelcomeMessage
        {
            FullName = fullName,
            Email = email,
            Id = id
        };
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        await _services.SendMessageAsync(message, QueuesNames.WelcomeQueue);
    }
    /// <summary>
    /// Sen Password recuperation message to email
    /// </summary>
    /// <param name="email"></param>
    /// <param name="token"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task PasswordRecuperationMessage(string email, string token, int id)
    {
        var message = new EmailVerifcationMessage { Email = email, Token = token, Id = id };
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        await this._services.SendMessageAsync(message, QueuesNames.PasswordRecuperationQeue);
    }
}