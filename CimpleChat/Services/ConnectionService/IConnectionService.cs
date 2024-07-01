using System.Net.WebSockets;

namespace CimpleChat.Services.ConnectionService
{
    public interface IConnectionService
    {
        public void AddConnection(long userId, WebSocket ws);

        public void RemoveConnection(long userId);

        public void RemoveConnection(WebSocket ws);
    }
}
