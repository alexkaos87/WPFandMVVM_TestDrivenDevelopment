using FriendStorage.DataAccess;
using FriendStorage.Model;
using System;
using System.Collections.Generic;

namespace FriendStorage.UI.DataProvider
{
    class NavigationDataProvider : INavigationDataProvider
    {
        private readonly Func<IDataService> _dataServiceCreator;

        public NavigationDataProvider(Func<IDataService> dataServiceCreator) => _dataServiceCreator = dataServiceCreator;
        
        public virtual IEnumerable<LookupItem> GetAllFriends()
        {
            using (var dataService = _dataServiceCreator())
            {
                return dataService.GetAllFriends();
            }
        }
    }
}
