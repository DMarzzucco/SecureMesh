using NewEmailVerificationWorker.Handlers;
using UserVerificationMessage;
using RabbitWorkerCore;

class Program
{
    static async Task Main(string[] args)
    {
        var handler = new EmailVerificationHandler();
        var worker = new WorkerConsumitor<UserMessage>("new_email_verification", handler);

        await worker.StartAsync();
    }
}