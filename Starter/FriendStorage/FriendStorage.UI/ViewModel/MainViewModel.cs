﻿using FriendStorage.Model;
using FriendStorage.UI.Command;
using FriendStorage.UI.Events;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace FriendStorage.UI.ViewModel
{
  public class MainViewModel : ViewModelBase
    {
        private IFriendEditViewModel _selectedFriendEditViewModel;
        
        private readonly Func<IFriendEditViewModel> _friendEditVmCreator;

        public MainViewModel(INavigationViewModel navigationViewModel,
            Func<IFriendEditViewModel> friendEditVmCreator,
            IEventAggregator eventAggregator)
        {
            NavigationViewModel = navigationViewModel;
            FriendEditViewModels = new ObservableCollection<IFriendEditViewModel>();
            _friendEditVmCreator = friendEditVmCreator;

            eventAggregator.GetEvent<OpenFriendEditViewEvent>().Subscribe(OnOpenFriendEditView);
            eventAggregator.GetEvent<FriendDeletedEvent>().Subscribe(OnFriendDeleted);

            CloseFriendTabCommand = new DelegateCommand(OnCloseFriendTabExecute);
            AddFriendCommand = new DelegateCommand(OnAddFriendExecute);
        }

        public ICommand CloseFriendTabCommand { get; private set; }

        public ICommand AddFriendCommand { get; private set; }

        public INavigationViewModel NavigationViewModel { get; private set; }

        public ObservableCollection<IFriendEditViewModel> FriendEditViewModels { get; private set; }

        public IFriendEditViewModel SelectedFriendEditViewModel
        {
            get => _selectedFriendEditViewModel;

            set
            {
                _selectedFriendEditViewModel = value;
                OnPropertyChanged();
            }
        }

        public void Load() => NavigationViewModel.Load();

        private void OnOpenFriendEditView(int friendId)
        {
            var friendEditVm = FriendEditViewModels.SingleOrDefault(vm => vm.Friend.Id == friendId);
            if (friendEditVm == null)
            {
                friendEditVm = CreateAndLoadFriendEditViewModel(friendId);
            }
            SelectedFriendEditViewModel = friendEditVm;
        }

        private void OnCloseFriendTabExecute(object obj)
        {
            if (obj is IFriendEditViewModel friendEditVm)
            {
                FriendEditViewModels.Remove(friendEditVm);
            }
        }

        private void OnAddFriendExecute(object obj) => SelectedFriendEditViewModel = CreateAndLoadFriendEditViewModel(null);

        private IFriendEditViewModel CreateAndLoadFriendEditViewModel(int? friendId)
        {
            var friendEditVm = _friendEditVmCreator();
            FriendEditViewModels.Add(friendEditVm);
            friendEditVm.Load(friendId);
            return friendEditVm;
        }

        private void OnFriendDeleted(int friendId)
        {
            var deletedFriendEditVm = FriendEditViewModels.Single(x => x.Friend.Id == friendId);
            FriendEditViewModels.Remove(deletedFriendEditVm);
        }
    }
}
