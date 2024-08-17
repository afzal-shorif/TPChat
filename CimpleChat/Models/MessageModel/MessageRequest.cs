namespace CimpleChat.Models.MessageModel;
public class MessageRequest
{
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    public string Content { get; set; }
    public long TempMessageId { get; set; }
    public DateTime? CreatedAt { get; set; }
}