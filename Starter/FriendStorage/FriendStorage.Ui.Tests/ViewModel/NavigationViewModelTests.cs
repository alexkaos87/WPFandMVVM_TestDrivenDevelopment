using FriendStorage.Model;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.ViewModel;
using Moq;
using Prism.Events;
using Xunit;

namespace FriendStorage.Ui.Tests.ViewModel
{
    public class NavigationViewModelTests
    {
        private readonly NavigationViewModel _viewModel;

        public NavigationViewModelTests()
        {
            var eventAggregatorMock = new Mock<IEventAggregator>();
            var navigationDataProviderMock = new Mock<INavigationDataProvider>();
            navigationDataProviderMock.Setup(dp => dp.GetAllFriends())
              .Returns(
              new List<LookupItem> { 

                  new() { Id = 1, DisplayMember = "Julia" },
                  new() { Id = 2, DisplayMember = "Thomas" }
              });
            _viewModel = new NavigationViewModel(
              navigationDataProviderMock.Object,
              eventAggregatorMock.Object);
        }

        [Fact]
        public void ShouldLoadFriends()
        {
            _viewModel.Load();

            Assert.Equal(2, _viewModel.Friends.Count);

            var friend = _viewModel.Friends.SingleOrDefault(f => f.Id == 1);
            Assert.NotNull(friend);
            Assert.Equal("Julia", friend.DisplayMember);

            friend = _viewModel.Friends.SingleOrDefault(f => f.Id == 2);
            Assert.NotNull(friend);
            Assert.Equal("Thomas", friend.DisplayMember);
        }

        [Fact]
        public void ShouldLoadFriendsOnlyOnce()
        {
            _viewModel.Load();
            _viewModel.Load();

            Assert.Equal(2, _viewModel.Friends.Count);
        }
    }
}
