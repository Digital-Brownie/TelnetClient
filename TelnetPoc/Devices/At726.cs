using System.Text.RegularExpressions;

namespace TelnetPoc.Devices;

public class At726 : Ont
{
    private readonly string _username;
    private readonly string _password;

    public At726(string username, string password, string hostName, int port) : base(hostName, port)
    {
        _username = username;
        _password = password;
    }
    
    public override async Task<int> GetPortVlan(int? port)
    {
        Console.WriteLine("Fetching VLANs");
        
        if (!await EnsureConnection())
        {
            Console.WriteLine("Trying to reconnect");
            await Login();
        }

        await TelnetClient.Write("vlan list");

        var rawVlans = await TelnetClient.Read();

        Console.WriteLine(rawVlans);

        ParseVlans(rawVlans);

        return default;
    }

    public override Task<int> GetPortSpeeds(int? port)
    {
        throw new NotImplementedException();
    }
    public override async Task<IEnumerable<string>> GetDevicesConnected(int? port)
    {
        Console.WriteLine("Fetching Ports");
        
        if (!await EnsureConnection())
        {
            Console.WriteLine("Trying to reconnect");
            await Login();
        }

        await TelnetClient.Write("switch list port");

        var rawVlans = await TelnetClient.Read();

        Console.WriteLine(rawVlans);

        return Array.Empty<string>();
    }

    public override Task<IEnumerable<string>> GetPortMacAddresses(int? port)
    {
        throw new NotImplementedException();
    }
    private async Task Login(bool attemptReconnect = false)
    {
        Console.WriteLine("Logging in...");
        if (attemptReconnect)
        {
            Console.WriteLine("Attempting to reconnect");
            await EnsureConnection();
        }

        var prompt = await TelnetClient.Read();

        Console.WriteLine(prompt);

        await TelnetClient.Write(_username);

        var usernameSent = await TelnetClient.Read();

        Console.WriteLine(usernameSent);

        await TelnetClient.Write(_password);
        
        var passwordSent = await TelnetClient.Read();
        Console.WriteLine(passwordSent);

        if (!TelnetClient.Connected)
        {
            throw new Exception("Couldn't login");
        }

        Console.WriteLine("logged in");
    }
    
}