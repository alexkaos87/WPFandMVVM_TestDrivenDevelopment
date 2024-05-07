using FluentAssertions;
using FriendStorage.Model;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.Events;
using FriendStorage.UI.ViewModel;
using Moq;
using Prism.Events;
using Xunit;

namespace FriendStorage.Ui.Tests.ViewModel
{
    public class NavigationViewModelTests
    {
        private readonly NavigationViewModel _viewModel;
        private readonly FriendSavedEvent _friendSavedEvent;

        public NavigationViewModelTests()
        {
            _friendSavedEvent = new FriendSavedEvent();

            var eventAggregatorMock = new Mock<IEventAggregator>();

            eventAggregatorMock.Setup(ea => ea.GetEvent<FriendSavedEvent>())
              .Returns(_friendSavedEvent);

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

        [Fact]
        public void ShouldUpdateNavigationItemWhenFriendIsSaved()
        {
            _viewModel.Load();
            var navigationItem = _viewModel.Friends.First();

            var friendId = navigationItem.Id;

            _friendSavedEvent.Publish(
              new Friend
              {
                  Id = friendId,
                  FirstName = "Anna",
                  LastName = "Huber"
              });

            navigationItem.DisplayMember.Should().Be("Anna Huber");
        }
    }
}
