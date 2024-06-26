﻿using FriendStorage.Model;
using FriendStorage.UI.Command;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.Events;
using FriendStorage.UI.Wrapper;
using Prism.Events;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace FriendStorage.UI.ViewModel
{
    public class FriendEditViewModel : ViewModelBase, IFriendEditViewModel
    {
        private readonly IFriendDataProvider _dataProvider;
        private readonly IEventAggregator _eventAggregator;
        private FriendWrapper _friend;

        public FriendEditViewModel(IFriendDataProvider dataProvider, IEventAggregator eventAggregator)
        {
            _dataProvider = dataProvider;
            _eventAggregator = eventAggregator;

            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute, OnDeleteCanExecute);
        }

        public ICommand SaveCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public virtual FriendWrapper Friend
        {
            get => _friend; 
            set
            {
                _friend = value;
                OnPropertyChanged();
            }
        }

        public virtual void Load(int? friendId)
        {
            // here we handle new friends that not exists yet
            var friend = friendId.HasValue ? _dataProvider.GetFriendById(friendId.Value) : new Friend();

            Friend = new FriendWrapper(friend);
            Friend.PropertyChanged += OnFriendPropertyChanged;
            InvalidateCommands();
        }

        private void OnFriendPropertyChanged(object sender, PropertyChangedEventArgs e) => InvalidateCommands();

        private void InvalidateCommands()
        {
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
        }

        private void OnSaveExecute(object obj)
        {
            _dataProvider.SaveFriend(Friend.Model);
            Friend.AcceptChanges();
            _eventAggregator.GetEvent<FriendSavedEvent>().Publish(Friend.Model);
        }

        private bool OnSaveCanExecute(object arg) => Friend != null && Friend.IsChanged;

        private void OnDeleteExecute(object obj)
        {
            _dataProvider.DeleteFriend(Friend.Id);
            _eventAggregator.GetEvent<FriendDeletedEvent>().Publish(Friend.Id);
        }

        private bool OnDeleteCanExecute(object arg) => Friend != null && Friend.Id > 0;
    }
}
