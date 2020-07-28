using System.Windows;
using System.Windows.Controls;
using TlbbGmTool.Models;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Windows
{
    public partial class ServerListWindow : Window
    {
        public ServerListWindow(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            GetViewModel().InitData(mainWindowViewModel);
        }

        private ServerListViewModel GetViewModel()
        {
            return DataContext as ServerListViewModel;
        }

        private void ShowEditServerDialog(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var serverInfo = btn.DataContext as GameServer;
            var editServerWindow = new AddOrEditServerWindow(serverInfo, GetViewModel().MainWindowViewModel)
            {
                Owner = this
            };
            editServerWindow.ShowDialog();
        }

        private void ShowAddServerDialog(object sender, RoutedEventArgs e)
        {
            var editServerWindow = new AddOrEditServerWindow(null, GetViewModel().MainWindowViewModel)
            {
                Owner = this
            };
            editServerWindow.ShowDialog();
        }

        private void ProcessDeleteServer(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var serverInfo = btn.DataContext as GameServer;
            //删除确认
            if (MessageBox.Show(this, $"你确定要删除服务器{serverInfo.ServerName}吗?",
                "删除提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            GetViewModel().ProcessDeleteServer(serverInfo);
        }
    }
}