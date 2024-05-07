using FriendStorage.UI.DataProvider;
using Prism.Events;
using System.Collections.ObjectModel;

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
        }
        public ObservableCollection<NavigationItemViewModel> Friends { get; private set; }

        public void Load()
        {
            Friends.Clear();
            foreach (var friend in _dataProvider.GetAllFriends())
            {
                Friends.Add(new NavigationItemViewModel(
                  friend.Id, friend.DisplayMember, _eventAggregator));
            }
        }
    }
}
