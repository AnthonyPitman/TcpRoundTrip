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
            System.Console.WriteLine("Client launching...");
        }

        if (appType == ApplicationType.server)
        {
            System.Console.WriteLine("Server launching...");
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

public enum ApplicationType
{
    client,
    server,
    invalid
}
