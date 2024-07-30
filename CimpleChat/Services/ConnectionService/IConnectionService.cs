using CimpleChat.Models;
using System.Net.WebSockets;

namespace CimpleChat.Services.ConnectionService
{
    public interface IConnectionService
    {
        Connection? GetConnection(long userId);
        IList<Connection> GetConnections(long channelId);
        IList<Connection> GetConnections(IList<long> users);

        void AddConnection(long userId, WebSocket ws);

        void RemoveConnection(long userId);

        void RemoveConnection(WebSocket ws);
    }
}
