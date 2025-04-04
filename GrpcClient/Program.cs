// See https://aka.ms/new-console-template for more information


using Grpc;
using Grpc.Core;
using Grpc.Net.Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7051");
        var metrics = new Greeter.GreeterClient(channel);

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
                default:
                    {
                        var response = await metrics.GetMetricsAsync(new MetrixRequest());
                        Console.WriteLine($"CPU Usage: {response.CpuUsage}%");
                        Console.WriteLine($"Available Memory: {response.AvailableMemoryMb} MB / {response.TotalMemoryMb} MB");
                        Console.WriteLine($"Disk Space: {response.FreeDiskSpaceGb} GB / {response.TotalDiskSpaceGb} GB");

                        break;
                    }
            }
        }
    }
}
