using FriendStorage.DataAccess;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.View;
using FriendStorage.UI.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Prism.Events;
using System;
using System.Windows;

namespace FriendStorage.UI
{
  public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddTransient<MainWindow>();

            services.AddTransient<MainViewModel>();

            services.AddTransient<INavigationViewModel, NavigationViewModel>();
            services.AddTransient<INavigationDataProvider, NavigationDataProvider>();
            services.AddSingleton<IEventAggregator, EventAggregator>();

            services.AddTransient<IDataService, FileDataService>();
            services.AddSingleton(provider => new Func<IDataService>(() => provider.GetService<IDataService>()));

            services.AddTransient<IFriendEditViewModel, FriendEditViewModel>();
            services.AddTransient<IFriendDataProvider, FriendDataProvider>();
            services.AddSingleton(provider => new Func<IFriendEditViewModel>(() => provider.GetService<IFriendEditViewModel>()));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}
