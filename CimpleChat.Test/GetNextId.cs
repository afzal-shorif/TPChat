using CimpleChat.Services;
using CimpleChat.Test.Infrastructure;
using Xunit;

namespace CimpleChat.Test
{
    public class TestGetNextId
    {
        [Fact]
        public void TestSingleUserId()
        {
            IGetNextId nextId = Helper.GetRequiredService<IGetNextId>();
            long id = nextId.GetUserId();
            Assert.True(id > 0);
            Assert.IsType<long>(id);
        }

        [Fact]
        public void TestSingleChannelId()
        {
            IGetNextId nextId = Helper.GetRequiredService<IGetNextId>();
            long id = nextId.GetChannelId();
            Assert.True(id > 0);
            Assert.IsType<long>(id);
        }

        [Fact]
        public void TestSingleMessageId()
        {
            IGetNextId nextId = Helper.GetRequiredService<IGetNextId>();
            long id = nextId.GetMessageId();
            Assert.False(id <= 0);
            Assert.IsType<long>(id);
        }

        [Fact]
        public void TestMultipleUserId()
        {
            IGetNextId nextId = Helper.GetRequiredService<IGetNextId>();
            List<long> ids = new List<long>();
            for(int i = 0; i < 255; i++)
            {
                long id = nextId.GetUserId();
                ids.Add(id);
            }

            List<long> d = ids.Distinct().ToList<long>();
            
            Assert.True(ids.Count == d.Count);
        }

        [Fact]
        public void TestMultipleChannelId()
        {
            IGetNextId nextId = Helper.GetRequiredService<IGetNextId>();
            List<long> ids = new List<long>();
            for (int i = 0; i < 255; i++)
            {
                long id = nextId.GetChannelId();
                ids.Add(id);
            }

            List<long> d = ids.Distinct().ToList<long>();

            Assert.True(ids.Count == d.Count);
        }

        [Fact]
        public void TestMultipleMessageId()
        {
            IGetNextId nextId = Helper.GetRequiredService<IGetNextId>();
            List<long> ids = new List<long>();
            for (int i = 0; i < 255; i++)
            {
                long id = nextId.GetMessageId();
                ids.Add(id);
            }

            List<long> d = ids.Distinct().ToList<long>();

            Assert.True(ids.Count == d.Count);
        }
    }
}
