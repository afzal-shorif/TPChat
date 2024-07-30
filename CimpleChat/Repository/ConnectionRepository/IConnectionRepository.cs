using CimpleChat.Models;
using System.Net.WebSockets;

namespace CimpleChat.Repository.ConnectionRepository;

public interface IConnectionRepository
{
    Connection? GetConnection(long userId);
    IList<Connection> GetConnections(IList<long> users);

    void AddConnection(long userId, WebSocket ws);

    void RemoveConnection(long userId);

    void RemoveConnection(WebSocket ws);
}