using System;
using System.Windows;
using TlbbGmTool.View.Windows;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private MainWindowViewModel GetViewModel()
        {
            return DataContext as MainWindowViewModel;
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var vm = GetViewModel();
            try
            {
                await vm.LoadApplicationData();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message, "出错了", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowServerListWindow(object sender, RoutedEventArgs e)
        {
            var serverListWindow = new ServerListWindow(GetViewModel())
            {
                Owner = this
            };
            serverListWindow.ShowDialog();
        }

        private void DoExit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ShowAboutWindow(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow()
            {
                Owner = this
            };
            aboutWindow.ShowDialog();
        }
    }
}