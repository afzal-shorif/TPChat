using CimpleChat.Models;
using System.Net.WebSockets;

namespace CimpleChat.Repository.ConnectionRepository;

public interface IConnectionRepository
{
    public void AddConnection(long userId, WebSocket ws);

    public void RemoveConnection(long userId);

    public void RemoveConnection(WebSocket ws);
}