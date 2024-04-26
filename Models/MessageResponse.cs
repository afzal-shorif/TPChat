namespace CimpleChat.Models
{
    public class MessageResponse
    {
        public string Type { get; set; } = "Message";
        public Message Message { get; set; }
        public User User { get; set; }
    }
}
