using UserVerificationMessage;
using RabbitWorkerCore;

namespace PasswordRecuperation.Handlers
{
    class PasswordRecuperationHandler : IMessageHandler<UserMessage>
    {
        public Task HandleAsync(UserMessage message)
        {
            Console.WriteLine($"Recuperacion de cuenta de {message.Email} con enlace: https://localhost:8888/api/Security/8382fd_1231sfw13312saeDAs12?hmk12={message.Token}");
            return Task.CompletedTask;
        }
    }
}