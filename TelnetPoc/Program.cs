using System.Net.Sockets;
using System.Text;
using TelnetPoc.Devices;

namespace TelnetPoc;

public static class Program
{
    private static OntFactory _factory = new();
    public static async Task Main(string[] args)
    {
        await using var ont = new At726("manager","friend","dev.nms-proxy.gcpud.aex.blue", 1174);
        
        await ont.GetPortVlan(1);

        // await ont.GetDevicesConnected(1);

    }

    private static async Task Testing()
    {
        Console.WriteLine($"{Thread.CurrentThread.Name}");
        
        using var telnetClient = await _factory.GetOnt("dev.nms-proxy.gcpud.aex.blue", 1174);

        var initialResponse = await telnetClient.Read();

        Console.WriteLine(initialResponse);
        
        await telnetClient.Write("manager");
        
        var usernameResponse = await telnetClient.Read();
        
        Console.WriteLine(usernameResponse);
        
        await telnetClient.Write("friend");
        
        var passwordResponse = await telnetClient.Read();
        
        Console.WriteLine(passwordResponse);

        await telnetClient.Write("switch list port");

        var ports = await telnetClient.Read();

        Console.WriteLine(ports);
        
        await telnetClient.Write("vlan list");

        var vlans = await telnetClient.Read();
        
        await telnetClient.Write("Sho mac add inter port1.0.x");

        var macs = await telnetClient.Read();

        Console.WriteLine(macs);
    }

    private static async Task TestGetBytes()
    {
        using TelnetRepository telnetClient = new("dev.nms-proxy.gcpud.aex.blue", 1174);
        
        await Task.Delay(1000);
        var sb = new StringBuilder();
        do
        {
            var @byte = telnetClient.ReadByte();
            Console.Write($"{@byte}: ");
            char c = (char)@byte;
            Console.WriteLine(c);
            sb.Append(c);
            if (Console.ReadKey().Key == ConsoleKey.UpArrow)
            {
                break;
            }
            
            
        } while (telnetClient.BytesAvailable > 0);

        Console.WriteLine(sb);
    }
}