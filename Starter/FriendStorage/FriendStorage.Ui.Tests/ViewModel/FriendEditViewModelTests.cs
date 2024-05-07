using FluentAssertions;
using FriendStorage.Model;
using FriendStorage.Ui.Tests.Extensions;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.ViewModel;
using Moq;
using Xunit;

namespace FriendStorage.Ui.Tests.ViewModel
{
    public class FriendEditViewModelTests
    {
        private const int _friendId = 5;
        private readonly Mock<IFriendDataProvider> _dataProviderMock;
        private readonly FriendEditViewModel _viewModel;

        public FriendEditViewModelTests()
        {
            _dataProviderMock = new Mock<IFriendDataProvider>();
            _dataProviderMock.Setup(dp => dp.GetFriendById(_friendId))
              .Returns(new Friend { Id = _friendId, FirstName = "Thomas" });

            _viewModel = new FriendEditViewModel(_dataProviderMock.Object);
        }

        [Fact]
        public void ShouldLoadFriend()
        {
            _viewModel.Load(_friendId);

            _viewModel.Friend.Should().NotBeNull();
            Assert.NotNull(_viewModel.Friend);
            _viewModel.Friend.Id.Should().Be(_friendId);

            _dataProviderMock.Verify(dp => dp.GetFriendById(_friendId), Times.Once);
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventForFriend()
        {
            var isFired = _viewModel.IsPropertyChangedFired(
              () => _viewModel.Load(_friendId),
              nameof(_viewModel.Friend));

            isFired.Should().BeTrue();
        }
    }
}
