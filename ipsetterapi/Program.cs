using ipsetterapi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWindowsService(options => options.ServiceName = "IP setter api");

var ifname = builder.Configuration.GetValue<string>("Interface");
var app = builder.Build();

//app.MapGet("/", (ILoggerFactory loggerFactory) => {
//    var logger = loggerFactory.CreateLogger("Start");
//    logger.LogInformation("Starting...");
//    return "Logging at work!";
//});

app.MapGet("/test", (ILoggerFactory loggerFactory, string? ip, string? nm, string? gw) =>
{
    var logger = loggerFactory.CreateLogger("API");
    if (ip == null || nm == null) return "parameter missing";

    var state = Setter.SetIp(ifname ?? "eth0", $"{ip} {nm} {gw ?? ""}");
    return state == 0 ? "ok" : "failed";
});


app.Run();


