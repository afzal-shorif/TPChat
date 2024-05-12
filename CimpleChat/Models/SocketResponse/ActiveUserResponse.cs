// This model will be used as web socket response of Active User List to a channel
// Response WebSocketResponse<IList<ActiveUserResponse>>()

namespace CimpleChat.Models.SocketResponse
{   
    public class ActiveUserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
