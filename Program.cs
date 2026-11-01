using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpRoundTrip;

class Program
{
    static async Task<int> Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Input must be either 'server' or 'client'.");
            return -1;
        }

        var appType = ParseApplicationType(args[0]);
        if (appType == ApplicationType.invalid)
        {
            Console.WriteLine($"The application type '{args[0]}' was not recognized. Please enter 'server' or 'client'");
        }

        if (appType == ApplicationType.client)
        {

            await Client.RunAsync();
        }

        if (appType == ApplicationType.server)
        {
            await Server.RunAsync();
        }

        return 0;
    }

    private static ApplicationType ParseApplicationType(string input)
    {
        if (input.Equals("SERVER", StringComparison.InvariantCultureIgnoreCase))
        {
            return ApplicationType.server;
        }

        if (input.Equals("CLIENT", StringComparison.InvariantCultureIgnoreCase))
        {
            return ApplicationType.client;
        }

        return ApplicationType.invalid;
    }
}

public class Client
{
    public static async Task RunAsync(int portNumber = 5050)
    {
        try
        {
            using TcpClient tcpClient = new();
            await tcpClient.ConnectAsync(IPAddress.Loopback, portNumber);
            await using NetworkStream stream = tcpClient.GetStream();
            using StreamReader reader = new(stream, Encoding.UTF8);
            using StreamWriter writer = new(stream, Encoding.UTF8)
            {
                AutoFlush = true
            };

            while (true)
            {
                Console.Write("Enter text or '/quit' to exit: ");
                string? userInput = Console.ReadLine();
                if (userInput is null || userInput == "/quit")
                {
                    Console.WriteLine("Client has quit.");
                    return;
                }

                await writer.WriteLineAsync(userInput);
                Console.WriteLine($"Sent: {userInput}");

                string? acknowlegement = await reader.ReadLineAsync();
                if (acknowlegement is null)
                {
                    Console.WriteLine("Server disconnected. Quitting.");
                    return;
                }

                Console.WriteLine($"Server responded: {acknowlegement}");
            }
        }
        catch (SocketException)
        {
            Console.WriteLine("Could not connect. Start the server first.");
        }
    }
}

public class Server
{
    public static async Task RunAsync(int portNumber = 5050)
    {
        TcpListener? server = null;

        try
        {
            server = new TcpListener(IPAddress.Loopback, portNumber);
            server.Start();
            Console.WriteLine("Server started");

            using TcpClient client = await server.AcceptTcpClientAsync();
            Console.WriteLine("Client connected!");
            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new(stream, Encoding.UTF8);
            using StreamWriter writer = new(stream, Encoding.UTF8)
            {
                AutoFlush = true
            };

            while (true)
            {
                var line = await reader.ReadLineAsync();
                if (line is null) break;
                Console.WriteLine("Received: {0}", line);

                string acknowlegement = $"ACK: {line}";

                await writer.WriteLineAsync(acknowlegement);
                Console.WriteLine($"Sent: {acknowlegement}");
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            server?.Stop();
            server?.Dispose();
        }
    }
}

public enum ApplicationType
{
    client,
    server,
    invalid
}
