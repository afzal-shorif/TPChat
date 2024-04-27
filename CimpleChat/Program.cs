using CimpleChat.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("https://localhost:6969");
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();

builder.Services.AddSingleton<IGetNextId, GetNextId>();
builder.Services.AddSingleton<IGroupMessageService, GroupMessageService>();
builder.Services.AddSingleton<IUserService, UserService>();

var app = builder.Build();

app.UseWebSockets();
app.UseRouting();
app.MapDefaultControllerRoute();
app.UseStaticFiles();

app.Run();
