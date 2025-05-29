using UserVerificationMessage;
using RabbitWorkerCore;
using PasswordRecuperation.Handlers;
class Program
{
    static async Task Main(string[] args)
    {
        var handler = new PasswordRecuperationHandler();
        var worker = new WorkerConsumitor<UserMessage>("password_recuperation", handler);

        await worker.StartAsync();
    }
}