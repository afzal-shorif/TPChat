using System.Net.WebSockets;

namespace CimpleChat.Models;
public class Connections
{
    public long UserId { get; set; }
    public WebSocket connection { get; set; }
}