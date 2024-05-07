using FluentAssertions;
using Xunit;

namespace FriendStorage.Ui.Tests
{
    public class Foo
    {
        [Fact]
        public void Test1()
        {
            var var1 = true;
            var1.Should().BeTrue();
        }

        [Theory]
        [InlineData(5, 2, 3)]
        [InlineData(10, 3, 7)]
        public void Test2(in int expected, in int first, in int second) => (first + second).Should().Be(expected);
    }
}
