using CimpleChat.Models;
using CimpleChat.Repository.ConnectionRepository;
using CimpleChat.Services.ChannelService;
using System.Net.WebSockets;

namespace CimpleChat.Services.ConnectionService;
public class ConnectionService: IConnectionService
{
    #region Fields

    private readonly IConnectionRepository _connectionRepository;
    private readonly IGroupMessageService _channelService;

    #endregion

    #region Ctor

    public ConnectionService(IConnectionRepository connectionRepository, IGroupMessageService channelService)
    {
        _connectionRepository = connectionRepository;
        _channelService = channelService;
    }

    #endregion

    #region public Methods

    public Connection? GetConnection(long userId)
    {
        return _connectionRepository.GetConnection(userId);
    }

    public IList<Connection> GetConnections(long channelId)
    {
        IList<long> users = _channelService.GetChannelUsers(1);

        return _connectionRepository.GetConnections(users);
    }

    public IList<Connection> GetConnections(IList<long> users)
    {
        return _connectionRepository.GetConnections(users);
    }

    public void AddConnection(long userId, WebSocket ws)
    {
        var conn = _connectionRepository.GetConnection(userId);

        if(conn != null)
        {
            _connectionRepository.RemoveConnection(userId);
        }

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
