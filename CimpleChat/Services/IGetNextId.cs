namespace CimpleChat.Services
{
    public interface IGetNextId
    {
        public int GetUserId();
        public int GetChannelId();
        public int GetMessageId();
    }
}
