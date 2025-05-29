using RabbitWorkerCore;
using WelcomeMessageWorker.Model;

namespace WelcomeMessageWorker.Handlers
{
    public class WelcomeMessageHandler : IMessageHandler<WelcomeMessage>
    {
        public Task HandleAsync(WelcomeMessage message)
        {
            Console.WriteLine($"Welcome our system {message.FullName}. Now you can login in");
            return Task.CompletedTask;
        }
    }
}