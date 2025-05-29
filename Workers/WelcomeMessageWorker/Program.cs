using RabbitWorkerCore;
using WelcomeMessageWorker.Handlers;
using WelcomeMessageWorker.Model;


class Program
{
    static async Task Main(string[] args)
    {
        var handler = new WelcomeMessageHandler();
        var worker = new WorkerConsumitor<WelcomeMessage>("welcome_queue", handler);

        await worker.StartAsync();
    }
}