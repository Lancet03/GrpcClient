// See https://aka.ms/new-console-template for more information


using Grpc;
using Grpc.Core;
using Grpc.Net.Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7051");
        var client = new Greeter.GreeterClient(channel);

        using var call = client.SayHelloStream();
        var readTask = Task.Run(async () =>
        {
            await foreach (var resp in call.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine(resp.Message);
            }
        });

        var metrics = new SystemMetrics.SystemMetricsClient(channel);
        using var metricsCall = metrics.GetMetricsStream();
        var readMetricsTask = Task.Run(async () =>
        {
            await foreach (var response in metricsCall.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"CPU Usage: {response.CpuUsage}%");
                Console.WriteLine($"Available Memory: {response.AvailableMemoryMb} MB / {response.TotalMemoryMb} MB");
                Console.WriteLine($"Disk Space: {response.FreeDiskSpaceGb} GB / {response.TotalDiskSpaceGb} GB");
            }
        });

        Console.ReadKey();

        Console.ReadKey();

        bool continueInput = true;
        while (continueInput)
        {
            var result = Console.ReadLine();
            switch (result)
            {
                case "0":
                    {
                        continueInput = false;
                        break;
                    }
                case "m":
                    {
                        await metricsCall.RequestStream.WriteAsync(new MetrixRequest() { });
                        break;

                    }
                default:
                   { await call.RequestStream.WriteAsync(new HelloRequest() { Name = result });
                        break;
                    }

            }
        }

        await call.RequestStream.CompleteAsync();
        await metricsCall.RequestStream.CompleteAsync();
        await readTask;
    }
}
