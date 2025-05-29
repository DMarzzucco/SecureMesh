using RabbitWorkerCore;
using UserVerificationMessage;

namespace EmailVerificationWorker.Handlers
{
    public class EmailVerificationHandler : IMessageHandler<UserMessage>
    {
        public Task HandleAsync(UserMessage message)
        {
            Console.WriteLine($"Validacion de email a {message.Email} con enlace: https://localhost:8888/api/Security/12349smska_wqj1n234msm949401?kl124={message.Token}");
            return Task.CompletedTask;
        }
    }
}