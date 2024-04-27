namespace CimpleChat.Models
{
    public enum MessageStatus
    {
        Saved,
        Delivered,
        Seen
    }

    public class Message
    {
        public long Id { get; set; }
        public long From { get; set; }
        public string Content { get; set; }
        public MessageStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
