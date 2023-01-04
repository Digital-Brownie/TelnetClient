using System.Text.RegularExpressions;

namespace TelnetPoc.Devices;

public abstract class Ont : IAsyncDisposable, IDisposable
{
    protected TelnetRepository? TelnetClient;

    protected Ont(string hostName, int port = 22)
    {
        TelnetClient = new TelnetRepository(hostName, port);
    }

    protected virtual async Task<bool> EnsureConnection()
    {
        var wasConnected = TelnetClient.Connected;
        if (wasConnected)
        {
            return wasConnected;
        }

        Console.WriteLine("trying to connect");

        await TelnetClient.Connect();

        if (!TelnetClient.Connected)
        {
            throw new Exception("Can't connect");
        }

        return wasConnected;
    }

    public abstract Task<int> GetPortVlan(int? port);
    
    public abstract Task<int> GetPortSpeeds(int? port);
    
    public abstract Task<IEnumerable<string>> GetDevicesConnected(int? port);
    
    public abstract Task<IEnumerable<string>> GetPortMacAddresses(int? port);

    public virtual Dictionary<string,string> ParseVlans(string rawVlans)
    {
        var regex = new Regex("\\d+ \\| \\d+");

        var result = regex.Match(rawVlans);

        return new();
    }

    public void Dispose()
    {
        Console.WriteLine("Called Dispose");
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        Console.WriteLine("Called DisposeAsync");
        await DisposeAsyncCore().ConfigureAwait(false);

        Dispose(disposing: false);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            TelnetClient?.Dispose();
            TelnetClient = null;
        }
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (TelnetClient is not null)
        {
            await TelnetClient.Write("ul");
            TelnetClient.Dispose();
        }

        TelnetClient = null;
    }
}