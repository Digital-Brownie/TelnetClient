using System.Net.Sockets;

namespace TelnetPoc;

public class TelnetRepository : IDisposable
{
    private readonly string _hostName;
    private readonly int _port;
    private readonly TcpClient _tcpClient;
    private NetworkStream _stream;
    
    private readonly Func<byte[], int, int, string> _getString;
    private readonly Func<string, byte[]> _getBytes;

    public TelnetRepository(string hostName, int port, Encoding encoding = Encoding.Ascii)
    {
        _hostName = hostName;
        _port = port;
        _tcpClient = new();

        _getString = encoding switch
        {
            Encoding.Ascii => System.Text.Encoding.ASCII.GetString,
            Encoding.Utf8 => System.Text.Encoding.UTF8.GetString,
            _ => throw new Exception("WTF"),
        };

        _getBytes = encoding switch
        {
            Encoding.Ascii => System.Text.Encoding.ASCII.GetBytes,
            Encoding.Utf8 => System.Text.Encoding.UTF8.GetBytes,
            _ => throw new Exception("WTF"),
        };
    }

    public int BytesAvailable => _tcpClient.Available;

    public bool Connected => _tcpClient.Connected;

    public async Task<bool> Connect()
    {
        await _tcpClient.ConnectAsync(_hostName, _port);
        _stream = _tcpClient.GetStream();
        return _tcpClient.Connected;
    }

    public async Task<string> Read()
    {
        await Task.Delay(1000);
        // Console.WriteLine($"Bytes available {_tcpClient.Available}");
        
        var outputBytes = new byte[_tcpClient.Available];
        var readBytes = await _stream.ReadAsync(outputBytes);
        
        // Console.WriteLine($"readBytes: {readBytes}");
        return _getString(outputBytes, 0, readBytes);
    }

    public int ReadByte() => _stream.ReadByte();
    
    public void WriteByte(byte @byte) => _stream.WriteByte(@byte);

    public Task Write(string command)
    {
        var commandBytes = _getBytes($"{command}\r\n");
        return _stream.WriteAsync(commandBytes, 0, commandBytes.Length);
    }

    public enum Encoding
    {
        Ascii,
        Utf8
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _tcpClient.Dispose();
            _stream.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~TelnetRepository()
    {
        Dispose(false);
    }
}