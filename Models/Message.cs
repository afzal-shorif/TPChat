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
        public int Id { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public string Content { get; set; }
        public MessageStatus Satus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
