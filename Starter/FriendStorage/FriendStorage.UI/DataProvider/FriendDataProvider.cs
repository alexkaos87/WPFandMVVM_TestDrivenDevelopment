using FriendStorage.DataAccess;
using FriendStorage.Model;
using System;

namespace FriendStorage.UI.DataProvider
{
    public class FriendDataProvider : IFriendDataProvider
    {
        private readonly Func<IDataService> _dataServiceCreator;

        public FriendDataProvider(Func<IDataService> dataServiceCreator) => _dataServiceCreator = dataServiceCreator;

        public virtual void DeleteFriend(int id)
        {
            using (var dataService = _dataServiceCreator())
            { 
                dataService.DeleteFriend(id);
            }
        }

        public virtual Friend GetFriendById(int id)
        {
            using (var dataService = _dataServiceCreator())
            {     
                return dataService.GetFriendById(id);
            }
        }

        public virtual void SaveFriend(Friend friend)
        {
            using (var dataService = _dataServiceCreator())
            { 
                dataService.SaveFriend(friend);
            }
        }
    }
}