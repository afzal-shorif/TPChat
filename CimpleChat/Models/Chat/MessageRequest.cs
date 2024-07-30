namespace CimpleChat.Models.Chat;
public class MessageRequest
{
    public long ChannelId { get; set; }
    public long UserId { get; set; }
    public string Content { get; set; }
    public long TempMessageId { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class MessageResponse
{
    public long MessageId { get; set; }
    public long UserId { get; set; }
    public string? UserName { get; set; }
    public string Content { get; set; }
    public Models.MessageStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MessageStatus
{
    public long ChannelId { get; set; }
    public long MessageId { get; set; }
    public long? TempMessageId { get; set; }
    public Models.MessageStatus Status { get; set; }
}

public class MessageHistoryRequest
{
    public long ChannelId { get; set; }
    public long LastMessageId { get; set; }
    public string RequestFor { get; set; }      // Latest: for new message, Old: for previous message

}