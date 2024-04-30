using CimpleChat.Services;
using Xunit;

namespace CimpleChat.Test
{
    public class TestGetNextId
    {
        [Fact]
        public void TestSingleUserId()
        {
            IGetNextId nextId = Helper.GetRequiredService<IGetNextId>();
            int id = nextId.GetUserId();
            Assert.True(id > 0);
            Assert.IsType<int>(id);
        }

        [Fact]
        public void TestSingleChannelId()
        {
            IGetNextId nextId = Helper.GetRequiredService<IGetNextId>();
            int id = nextId.GetChannelId();
            Assert.True(id > 0);
            Assert.IsType<int>(id);
        }

        [Fact]
        public void TestSingleMessageId()
        {
            IGetNextId nextId = Helper.GetRequiredService<IGetNextId>();
            int id = nextId.GetMessageId();
            Assert.False(id <= 0);
            Assert.IsType<int>(id);
        }

        [Fact]
        public void TestMultipleUserId()
        {
            IGetNextId nextId = Helper.GetRequiredService<IGetNextId>();
            List<int> ids = new List<int>();
            for(int i = 0; i < 255; i++)
            {
                int id = nextId.GetUserId();
                ids.Add(id);
            }

            List<int> d = ids.Distinct().ToList<int>();
            
            Assert.True(ids.Count == d.Count);
        }

        [Fact]
        public void TestMultipleChannelId()
        {
            IGetNextId nextId = Helper.GetRequiredService<IGetNextId>();
            List<int> ids = new List<int>();
            for (int i = 0; i < 255; i++)
            {
                int id = nextId.GetChannelId();
                ids.Add(id);
            }

            List<int> d = ids.Distinct().ToList<int>();

            Assert.True(ids.Count == d.Count);
        }

        [Fact]
        public void TestMultipleMessageId()
        {
            IGetNextId nextId = Helper.GetRequiredService<IGetNextId>();
            List<int> ids = new List<int>();
            for (int i = 0; i < 255; i++)
            {
                int id = nextId.GetMessageId();
                ids.Add(id);
            }

            List<int> d = ids.Distinct().ToList<int>();

            Assert.True(ids.Count == d.Count);
        }
    }
}
