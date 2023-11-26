using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ipsetter;

public class WindowsBackgroundService : BackgroundService
{
    private readonly ILogger<WindowsBackgroundService> _logger;
    private readonly IConfiguration _configuration;
    private TcpListener _listener = new(IPAddress.Loopback, 1234);

    public WindowsBackgroundService(ILogger<WindowsBackgroundService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;        
    }

    private int SetIp(string ifname, string parameters)
    {
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        startInfo.FileName = "cmd.exe";
        //startInfo.Arguments = "/C netsh interface ip set address name=eth3 static 192.168.5.10 255.255.255.0 192.168.5.1";
        startInfo.Arguments = $"/C netsh interface ip set address name={ifname} static {parameters}";
        startInfo.Verb = "runas";

        process.StartInfo = startInfo;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();        
        process.WaitForExit();
        return process.ExitCode;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            string? ifname = _configuration.GetValue<string>("Interface");
            int? ifport = _configuration.GetValue<int>("Port");
            if (ifname == null || ifport == null)
            {
                _logger.LogWarning("Configuration missing");
                return;
            }
            else
                _logger.LogWarning($"Start listen, interface: {ifname}, port: {ifport}");

            _listener = new(IPAddress.Loopback, ifport ?? 1234);
            _listener.Start();
            Byte[] bytes = new Byte[256];

            while (true)
            {
                _logger.LogWarning("Wait for connection");
                using TcpClient client = await _listener.AcceptTcpClientAsync(stoppingToken);

                NetworkStream stream = client.GetStream();
                stream.Write(Encoding.ASCII.GetBytes("ip nm gw\r\n"));
                int i = stream.Read(bytes, 0, bytes.Length);
                string pars = Encoding.ASCII.GetString(bytes, 0, i);
                _logger.LogWarning($"Message {pars}");
                stream.Write(Encoding.ASCII.GetBytes("ACK - "));
                int result = SetIp(ifname, pars);
                _logger.LogWarning($"netsh exit code: {result}");
                if (result == 0)
                {
                    stream.Write(Encoding.ASCII.GetBytes("OK\r\n"));
                }
                else
                {
                    stream.Write(Encoding.ASCII.GetBytes("ERROR\r\n"));
                }
                Task.Delay(500).Wait();
                stream.Close();
            }
        }
        catch (OperationCanceledException ex)
        {
            _listener.Stop();
            _logger.LogWarning(ex, "{Message}", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            Environment.Exit(1);
        }
    }
}
