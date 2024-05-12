namespace CimpleChat.Models.SocketResponse
{
    public class MessageResponse<T>
    {
        public string MessageType { get; set; } = "Single";
        public T MessageInfo { get; set; }
    }

    public class SingleMessageResponse
    {
        public Message Message { get; set; }
        public User User { get; set; }
    }

    public class AnnouncedMessageResponse
    {
        public string Content { get; set; }
    }
}
