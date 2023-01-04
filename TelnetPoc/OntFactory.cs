using System.Collections.Concurrent;

namespace TelnetPoc;

public class OntFactory
{
    private ConcurrentBag<string> _conntectedHosts = new ();

    public async Task<TelnetRepository> GetOnt(string hostName, int port = 22)
    {
        var attempts = 0;
        while (_conntectedHosts.Contains(hostName) && attempts < 3)
        {
            attempts++;
            Console.WriteLine("Waiting for host release");
            await Task.Delay(1000);
        }
        
        _conntectedHosts.Add(hostName);

        return new TelnetRepository(hostName, port);
    }
}