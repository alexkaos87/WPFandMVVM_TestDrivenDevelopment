using FriendStorage.UI.ViewModel;
using System.Windows;

namespace FriendStorage.UI.View
{
    public partial class MainWindow : Window
  {
        private readonly MainViewModel _viewModel;

        public MainWindow(MainViewModel viewModel)
        {
            _viewModel = viewModel;

            InitializeComponent();
            DataContext = _viewModel;
            Loaded += MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e) => _viewModel.Load();
    }
}
