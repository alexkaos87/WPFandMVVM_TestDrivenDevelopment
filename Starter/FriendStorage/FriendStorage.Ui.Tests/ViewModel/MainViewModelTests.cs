﻿using FluentAssertions;
using FriendStorage.Model;
using FriendStorage.Ui.Tests.Extensions;
using FriendStorage.UI.Events;
using FriendStorage.UI.ViewModel;
using FriendStorage.UI.Wrapper;
using Moq;
using Prism.Events;
using Xunit;

#nullable disable

namespace FriendStorage.Ui.Tests.ViewModel
{
    public class MainViewModelTests
    {
        private readonly Mock<INavigationViewModel> _navigationViewModelMock;
        private readonly MainViewModel _viewModel;
        private readonly Mock<IEventAggregator> _eventAggregatorMock;
        private readonly OpenFriendEditViewEvent _openFriendEditViewEvent;
        private readonly FriendDeletedEvent _friendDeletedEvent;
        private readonly List<Mock<IFriendEditViewModel>> _friendEditViewModelMocks;

        public MainViewModelTests()
        {
            _friendEditViewModelMocks = new List<Mock<IFriendEditViewModel>>();
            _navigationViewModelMock = new Mock<INavigationViewModel>();

            _openFriendEditViewEvent = new OpenFriendEditViewEvent();
            _friendDeletedEvent = new FriendDeletedEvent();

            _eventAggregatorMock = new Mock<IEventAggregator>();
            _eventAggregatorMock.Setup(ea => ea.GetEvent<OpenFriendEditViewEvent>())
              .Returns(_openFriendEditViewEvent);
            _eventAggregatorMock.Setup(ea => ea.GetEvent<FriendDeletedEvent>())
              .Returns(_friendDeletedEvent);

            _viewModel = new MainViewModel(_navigationViewModelMock.Object,
              CreateFriendEditViewModel, _eventAggregatorMock.Object);
        }

        private IFriendEditViewModel CreateFriendEditViewModel()
        {
            var friendEditViewModelMock = new Mock<IFriendEditViewModel>();
            friendEditViewModelMock.Setup(vm => vm.Load(It.IsAny<int>()))
              .Callback<int?>(friendId => friendEditViewModelMock.Setup(vm => vm.Friend)
            .Returns(new FriendWrapper(new Friend { Id = friendId.Value })));
            _friendEditViewModelMocks.Add(friendEditViewModelMock);
            return friendEditViewModelMock.Object;
        }

        [Fact]
        public void ShouldCallTheLoadMethodOfTheNavigationViewModel()
        {
            _viewModel.Load();

            _navigationViewModelMock.Verify(vm => vm.Load(), Times.Once);
        }

        [Fact]
        public void ShouldAddFriendEditViewModelAndLoadAndSelectIt()
        {
            const int friendId = 7;
            _openFriendEditViewEvent.Publish(friendId);

            _viewModel.FriendEditViewModels.Should().ContainSingle();

            var friendEditVm = _viewModel.FriendEditViewModels.First();
            _viewModel.SelectedFriendEditViewModel.Should().Be(friendEditVm);

            _friendEditViewModelMocks.First().Verify(vm => vm.Load(friendId), Times.Once);
        }

        [Fact]
        public void ShouldAddFriendEditViewModelsOnlyOnce()
        {
            _openFriendEditViewEvent.Publish(5);
            _openFriendEditViewEvent.Publish(5);
            _openFriendEditViewEvent.Publish(6);
            _openFriendEditViewEvent.Publish(7);
            _openFriendEditViewEvent.Publish(7);

            _viewModel.FriendEditViewModels.Should().HaveCount(3);
        }

        [Fact]
        public void ShouldRaisePropertyChangedEventForSelectedFriendEditViewModel()
        {
            var friendEditVmMock = new Mock<IFriendEditViewModel>();
            var isFired = _viewModel.IsPropertyChangedFired(() => _viewModel.SelectedFriendEditViewModel = friendEditVmMock.Object, nameof(_viewModel.SelectedFriendEditViewModel));

            isFired.Should().BeTrue();
        }

        [Fact]
        public void ShouldRemoveFriendEditViewModelOnCloseFriendTabCommand()
        {
            _openFriendEditViewEvent.Publish(7);

            var friendEditVm = _viewModel.SelectedFriendEditViewModel;

            _viewModel.CloseFriendTabCommand.Execute(friendEditVm);

            _viewModel.FriendEditViewModels.Should().BeEmpty();
        }

        [Fact]
        public void ShouldAddFriendEditViewModelAndLoadItWithIdNullAndSelectIt()
        {
            _viewModel.AddFriendCommand.Execute(null);

            _viewModel.FriendEditViewModels.Should().ContainSingle();

            var friendEditVm = _viewModel.FriendEditViewModels.First();
            _viewModel.SelectedFriendEditViewModel.Should().Be(friendEditVm);

            _friendEditViewModelMocks.First().Verify(vm => vm.Load(null), Times.Once);
        }
        [Fact]
        public void ShouldRemoveFriendEditViewModelOnFriendDeletedEvent()
        {
            const int deletedFriendId = 7;

            _openFriendEditViewEvent.Publish(deletedFriendId);
            _openFriendEditViewEvent.Publish(8);
            _openFriendEditViewEvent.Publish(9);

            _friendDeletedEvent.Publish(deletedFriendId);

            _viewModel.FriendEditViewModels.Should().HaveCount(2);
            _viewModel.FriendEditViewModels.Should().NotContain(vm => vm.Friend.Id == deletedFriendId);
        }
    }
}
