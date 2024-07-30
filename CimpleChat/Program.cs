using CimpleChat.Infrastructure;
using CimpleChat.Services;
using CimpleChat.Services.ChannelService;
using CimpleChat.Services.UserService;
using CimpleChat.Services.SocketService;
using CimpleChat.BackgroundService;
using CimpleChat.Services.ConnectionService;
using CimpleChat.Repository.ConnectionRepository;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("https://localhost:6969");
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();

builder.Services.AddSingleton<IGetNextId, GetNextId>();
builder.Services.AddSingleton<IGroupMessageService, GroupMessageService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IConnectionService, ConnectionService>();

builder.Services.AddSingleton<IConnectionRepository, ConnectionRepository>();

builder.Services.AddSingleton<ChannelMessageHandler>();
builder.Services.AddHostedService<RefreshUserLastOn>();



var app = builder.Build();

app.UseWebSockets();
app.Map("/WebSocket", (_app) => 
{ 
    _app.UseMiddleware<WebSocketManagerMiddleware>(app.Services.GetRequiredService<ChannelMessageHandler>());
});
app.UseRouting();
app.MapDefaultControllerRoute();
app.UseStaticFiles();

app.Run();
