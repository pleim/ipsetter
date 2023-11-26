using ipsetter;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

// Project template
// https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service?pivots=dotnet-6-0
//
// How to install the service
// sc.exe create "IP setter service" binpath="C:\... ...\ipsetter.exe"
//
// Set interface and portnumber in appsettings.json
//

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "IP setter service";
    })
    .ConfigureServices(services =>
    {        
        LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(services);
        services.AddHostedService<WindowsBackgroundService>();
    })    
    .Build();

host.Run();
