using CimpleChat.Infrastructure;
using CimpleChat.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("https://localhost:6969");
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();

builder.Services.AddSingleton<IGetNextId, GetNextId>();
builder.Services.AddSingleton<IGroupMessageService, GroupMessageService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<ChannelMessageHandler>();

var app = builder.Build();

app.UseWebSockets();
app.Map("/Chat", (_app) => { _app.UseMiddleware<WebSocketManagerMiddleware>(app.Services.GetRequiredService<ChannelMessageHandler>()); });
app.UseRouting();
app.MapDefaultControllerRoute();
app.UseStaticFiles();

app.Run();
