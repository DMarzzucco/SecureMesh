using RabbitWorkerCore;
using UserVerificationMessage;
namespace NewEmailVerificationWorker.Handlers
{
    public class EmailVerificationHandler : IMessageHandler<UserMessage>
    {
        public Task HandleAsync(UserMessage message)
        {
            Console.WriteLine($"Actualizacion de email de {message.Email} con enlace: https://localhost:5090/api/Security/5413444_dsdn123fS_231_ddf?klt1276={message.Token}");
            return Task.CompletedTask;
        }
    }
}