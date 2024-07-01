namespace CimpleChat.Services
{
    public interface IGetNextId
    {
        public long GetUserId();
        public long GetChannelId();
        public long GetMessageId();
    }
}
