namespace CimpleChat.Models.MessageModel
{
    public class MessageStatusReqRes
    {
        public long ChannelId { get; set; }
        public long MessageId { get; set; }
        public long? TempMessageId { get; set; }
        public MessageStatus Status { get; set; }
    }
}
