using System.ComponentModel;

namespace FriendStorage.Ui.Tests.Extensions
{
    public static class NotifyPropertyChangedExtensions
    {
        public static bool IsPropertyChangedFired(this INotifyPropertyChanged notifyPropertyChanged, Action action, string propertyName)
        {
            var isFired = false;
            notifyPropertyChanged.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == propertyName)
                {
                    isFired = true;
                }
            };

            action();

            return isFired;
        }
    }
}
