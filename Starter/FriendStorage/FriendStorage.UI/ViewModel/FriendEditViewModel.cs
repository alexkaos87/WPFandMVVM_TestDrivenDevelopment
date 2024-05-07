using FriendStorage.Model;
using FriendStorage.UI.DataProvider;

namespace FriendStorage.UI.ViewModel
{
    public class FriendEditViewModel : ViewModelBase, IFriendEditViewModel
    {
        private readonly IFriendDataProvider _dataProvider;

        private Friend _friend;

        public FriendEditViewModel(IFriendDataProvider dataProvider) => _dataProvider = dataProvider;

        public virtual Friend Friend
        {
            get => _friend; 
            set
            {
                _friend = value;
                OnPropertyChanged();
            }
        }

        public virtual void Load(int friendId)
        {
            var friend = _dataProvider.GetFriendById(friendId);

            Friend = friend;
        }
    }
}
