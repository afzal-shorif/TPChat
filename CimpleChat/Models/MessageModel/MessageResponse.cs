namespace CimpleChat.Models.MessageModel
{
    public class MessageResponse
    {
        public long ChannelId { get; set; }
        public long MessageId { get; set; }
        public long UserId { get; set; }
        public string? UserName { get; set; }
        public string Content { get; set; }
        public MessageStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
