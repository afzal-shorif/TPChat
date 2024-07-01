// This model will be used as web socket response of Active User List to a channel
// Response WebSocketResponse<IList<ActiveUserResponse>>()

namespace CimpleChat.Models.Chat
{
    public class ActiveUserResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
