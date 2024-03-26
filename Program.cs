var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("https://localhost:6969");
builder.Services.AddControllersWithViews();

var app = builder.Build();
app.UseRouting();
app.MapDefaultControllerRoute();

app.Run();
