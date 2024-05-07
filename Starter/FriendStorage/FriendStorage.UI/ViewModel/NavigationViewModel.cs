using FriendStorage.Model;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.Events;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FriendStorage.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private readonly INavigationDataProvider _dataProvider;
        
        private readonly IEventAggregator _eventAggregator;

        public NavigationViewModel(INavigationDataProvider dataProvider, IEventAggregator eventAggregator)
        {
            Friends = new ObservableCollection<NavigationItemViewModel>();
            _dataProvider = dataProvider;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<FriendSavedEvent>().Subscribe(OnFriendSaved);
        }

        public ObservableCollection<NavigationItemViewModel> Friends { get; private set; }

        public virtual void Load()
        {
            Friends.Clear();
            foreach (var friend in _dataProvider.GetAllFriends())
            {
                Friends.Add(new NavigationItemViewModel(
                  friend.Id, friend.DisplayMember, _eventAggregator));
            }
        }

        private void OnFriendSaved(Friend friend)
        {
            var displayName = $"{friend.FirstName} {friend.LastName}";
            var navigationItem = Friends.SingleOrDefault(x => x.Id == friend.Id);
            if (navigationItem != null) 
            {
                navigationItem.DisplayMember = displayName;
            }
            else
            {
                navigationItem = new NavigationItemViewModel(friend.Id, displayName, _eventAggregator);
                Friends.Add(navigationItem);
            }
        }
    }
}
