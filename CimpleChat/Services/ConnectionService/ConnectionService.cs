using CimpleChat.Models;
using CimpleChat.Repository.ConnectionRepository;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Net.WebSockets;

namespace CimpleChat.Services.ConnectionService;
public class ConnectionService: IConnectionService
{
    #region Fields

    private readonly IConnectionRepository _connectionRepository;

    #endregion

    #region Ctor

    public ConnectionService(IConnectionRepository connectionRepository)
    {
        _connectionRepository = connectionRepository;
    }

    #endregion

    #region public Methods

    public void AddConnection(long userId, WebSocket ws)
    {
        _connectionRepository.AddConnection(userId, ws);
    }

    public void RemoveConnection(long userId)
    {
        _connectionRepository.RemoveConnection(userId);
    }

    public void RemoveConnection(WebSocket ws)
    {
        _connectionRepository.RemoveConnection(ws);
    }

    #endregion
}
