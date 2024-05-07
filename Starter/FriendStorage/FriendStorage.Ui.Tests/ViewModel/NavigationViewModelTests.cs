using FluentAssertions;
using FriendStorage.Model;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.Events;
using FriendStorage.UI.ViewModel;
using Moq;
using Prism.Events;
using Xunit;

#nullable disable

namespace FriendStorage.Ui.Tests.ViewModel
{
    public class NavigationViewModelTests
    {
        private readonly NavigationViewModel _viewModel;
        private readonly FriendSavedEvent _friendSavedEvent;
        private readonly FriendDeletedEvent _friendDeletedEvent;

        public NavigationViewModelTests()
        {
            _friendSavedEvent = new FriendSavedEvent();
            _friendDeletedEvent = new FriendDeletedEvent();

            var eventAggregatorMock = new Mock<IEventAggregator>();

            eventAggregatorMock.Setup(ea => ea.GetEvent<FriendSavedEvent>())
              .Returns(_friendSavedEvent);
            eventAggregatorMock.Setup(ea => ea.GetEvent<FriendDeletedEvent>())
              .Returns(_friendDeletedEvent);

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

        [Fact]
        public void ShouldAddNavigationItemWhenAddedFriendIsSaved()
        {
            _viewModel.Load();

            const int newFriendId = 97;

            _friendSavedEvent.Publish(new Friend
            {
                Id = newFriendId,
                FirstName = "Anna",
                LastName = "Huber"
            });

            _viewModel.Friends.Should().HaveCount(3);

            var addedItem = _viewModel.Friends.SingleOrDefault(f => f.Id == newFriendId);
            addedItem.Should().NotBeNull();
            addedItem.DisplayMember.Should().Be("Anna Huber");
        }

        [Fact]
        public void ShouldRemoveNavigationItemWhenFriendIsDeleted()
        {
            _viewModel.Load();

            var deletedFriendId = _viewModel.Friends.First().Id;

            _friendDeletedEvent.Publish(deletedFriendId);

            _viewModel.Friends.Should().ContainSingle();
            _viewModel.Friends.Single().Id.Should().NotBe(deletedFriendId);
        }
    }
}
