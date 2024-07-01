
using CimpleChat.Services.UserService;

namespace CimpleChat.BackgroundService;

public class RefreshUserLastOn : IHostedService, IDisposable
{
    private readonly IUserService _userService;
    private Timer? _timer;
    public RefreshUserLastOn(IUserService userService)
    {
        _userService = userService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(RemoveDeactiveUsers, null, TimeSpan.Zero, TimeSpan.FromMinutes(2));
        
        return Task.CompletedTask;
    }

    private void RemoveDeactiveUsers(object? state)
    {
        _userService.RemoveInactiveUsers();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
