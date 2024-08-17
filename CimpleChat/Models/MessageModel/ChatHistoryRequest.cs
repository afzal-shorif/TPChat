namespace CimpleChat.Models.MessageModel
{
    public class ChatHistoryRequest
    {
        public long ChannelId { get; set; }
        public long LastMessageId { get; set; }
        public string RequestFor { get; set; }      // Latest: for new message, Old: for previous message
    }
}
