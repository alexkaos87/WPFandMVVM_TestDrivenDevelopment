using FluentAssertions;
using FriendStorage.Model;
using FriendStorage.Ui.Tests.Extensions;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.Events;
using FriendStorage.UI.ViewModel;
using Moq;
using Prism.Events;
using Xunit;

namespace FriendStorage.Ui.Tests.ViewModel
{
    public class FriendEditViewModelTests
    {
        private const int _friendId = 5;
        private readonly Mock<FriendSavedEvent> _friendSavedEventMock;
        private readonly Mock<IEventAggregator> _eventAggregatorMock;
        private readonly Mock<IFriendDataProvider> _dataProviderMock;
        private readonly FriendEditViewModel _viewModel;

        public FriendEditViewModelTests()
        {
            _friendSavedEventMock = new Mock<FriendSavedEvent>();
            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<FriendSavedEvent>()).Returns(_friendSavedEventMock.Object);

            _dataProviderMock = new Mock<IFriendDataProvider>();
            _dataProviderMock.Setup(dp => dp.GetFriendById(_friendId))
              .Returns(new Friend { Id = _friendId, FirstName = "Thomas" });

            _viewModel = new FriendEditViewModel(_dataProviderMock.Object, _eventAggregatorMock.Object);
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

        [Fact]
        public void ShouldDisableSaveCommandWhenFriendIsLoaded()
        {
            _viewModel.Load(_friendId);

            _viewModel.SaveCommand.CanExecute(null).Should().BeFalse();
        }

        [Fact]
        public void ShouldEnableSaveCommandWhenFriendIsChanged()
        {
            _viewModel.Load(_friendId);

            _viewModel.Friend.FirstName = "Changed";

            _viewModel.SaveCommand.CanExecute(null).Should().BeTrue();
        }

        [Fact]
        public void ShouldDisableSaveCommandWithoutLoad()
        {            
            _viewModel.SaveCommand.CanExecute(null).Should().BeFalse();
        }

        [Fact]
        public void ShouldRaiseCanExecuteChangedForSaveCommandWhenFriendIsChanged()
        {
            _viewModel.Load(_friendId);
            var isFired = false;
            _viewModel.SaveCommand.CanExecuteChanged += (s, e) => isFired = true;
            _viewModel.Friend.FirstName = "Changed";
            isFired.Should().BeTrue();
        }

        [Fact]
        public void ShouldRaiseCanExecuteChangedForSaveCommandAfterLoad()
        {
            var isFired = false;
            _viewModel.SaveCommand.CanExecuteChanged += (s, e) => isFired = true;
            _viewModel.Load(_friendId);
            isFired.Should().BeTrue();
        }

        [Fact]
        public void ShouldCallSaveMethodOfDataProviderWhenSaveCommandIsExecuted()
        {
            _viewModel.Load(_friendId);
            _viewModel.Friend.FirstName = "Changed";

            _viewModel.SaveCommand.Execute(null);
            _dataProviderMock.Verify(dp => dp.SaveFriend(_viewModel.Friend.Model), Times.Once);
        }

        [Fact]
        public void ShouldAcceptChangesWhenSaveCommandIsExecuted()
        {
            _viewModel.Load(_friendId);
            _viewModel.Friend.FirstName = "Changed";

            _viewModel.SaveCommand.Execute(null);
            _viewModel.Friend.IsChanged.Should().BeFalse();
        }

        [Fact]
        public void ShouldPublishFriendSavedEventWhenSaveCommandIsExecuted()
        {
            _viewModel.Load(_friendId);
            _viewModel.Friend.FirstName = "Changed";

            _viewModel.SaveCommand.Execute(null);
            _friendSavedEventMock.Verify(e => e.Publish(_viewModel.Friend.Model), Times.Once);
        }

        [Fact]
        public void ShouldCreateNewFriendWhenNullIsPassedToLoadMethod()
        {
            _viewModel.Load(null);

            _viewModel.Friend.Should().NotBeNull();
            
            _viewModel.Friend.Id.Should().Be(0);
            _viewModel.Friend.FirstName.Should().BeNull();
            _viewModel.Friend.LastName.Should().BeNull();
            _viewModel.Friend.Birthday.Should().BeNull();
            _viewModel.Friend.IsDeveloper.Should().BeFalse();

            _dataProviderMock.Verify(dp => dp.GetFriendById(It.IsAny<int>()), Times.Never);
        }
    }
}
