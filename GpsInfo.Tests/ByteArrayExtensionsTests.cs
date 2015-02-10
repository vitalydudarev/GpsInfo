using Xunit;

namespace GpsInfo.Tests
{
    public class ByteArrayExtensionsTests
    {
        [Fact]
        public void ToStringTest()
        {
            var bytes = new byte[] { 0x59, 0x45, 0x53 };
            var result = bytes.ToString(false);

            Assert.Equal("YES", result);
        }
    }
}
