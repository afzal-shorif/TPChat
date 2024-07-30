﻿using CimpleChat.Models;
using System.Net.WebSockets;
using System.Text;

namespace CimpleChat.Services.SocketService
{
    public abstract class WebSocketHandler
    {
        public WebSocketHandler()
        {

        }

        public abstract Task OnConnectAsync(WebSocket ws, User user);

        public abstract Task OnDisconnectAsync(WebSocket ws, User user);

        public abstract Task ReceiveAsync(WebSocket ws, User user);


        public async ValueTask SendMessageAsync(IList<WebSocket> connections, string message)
        {
            byte[] msgByte = Encoding.UTF8.GetBytes(message);
            ArraySegment<byte> buffer = new ArraySegment<byte>(msgByte);

            foreach (WebSocket connection in connections)
            {

                if (connection.State == WebSocketState.Open)
                {
                    await connection.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public async ValueTask SendMessageAsync(WebSocket ws, string message)
        {
            byte[] msgByte = Encoding.UTF8.GetBytes(message);
            ArraySegment<byte> buffer = new ArraySegment<byte>(msgByte);

            if (ws.State == WebSocketState.Open)
            {
                await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
