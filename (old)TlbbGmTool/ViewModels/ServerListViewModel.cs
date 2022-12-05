using System;
using System.Collections.ObjectModel;
using System.Windows;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.Services;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class ServerListViewModel : BindDataBase
    {
        #region Fields

        private MainWindowViewModel _mainWindowViewModel;
        private ServerListWindow _serverListWindow;

        #endregion

        #region Properties

        public ObservableCollection<GameServer> ServerList { get; private set; }
            = new ObservableCollection<GameServer>();

        public AppCommand AddServerCommand { get; }
        public AppCommand EditServerCommand { get; }

        public AppCommand DeleteServerCommand { get; }

        #endregion

        public ServerListViewModel()
        {
            AddServerCommand = new AppCommand(ShowAddServerDialog);
            EditServerCommand = new AppCommand(ShowEditServerDialog, CanShowEditServerDialog);
            DeleteServerCommand = new AppCommand(ProcessDeleteServer, CanShowEditServerDialog);
        }

        public void InitData(MainWindowViewModel mainWindowViewModel, ServerListWindow serverListWindow)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _serverListWindow = serverListWindow;
            ServerList = mainWindowViewModel.ServerList;
            RaisePropertyChanged(nameof(ServerList));
        }

        private bool CanShowEditServerDialog(object parameter)
        {
            var serverInfo = parameter as GameServer;
            return !serverInfo.Connected;
        }

        private void ShowEditServerDialog(object parameter)
        {
            var serverInfo = parameter as GameServer;
            var editServerWindow = new AddOrEditServerWindow(serverInfo, _mainWindowViewModel)
            {
                Owner = _serverListWindow
            };
            editServerWindow.ShowDialog();
        }

        private void ShowAddServerDialog()
        {
            var editServerWindow = new AddOrEditServerWindow(null, _mainWindowViewModel)
            {
                Owner = _serverListWindow
            };
            editServerWindow.ShowDialog();
        }

        private async void ProcessDeleteServer(object parameter)
        {
            var serverInfo = parameter as GameServer;
            //删除确认
            if (MessageBox.Show(_serverListWindow, $"你确定要删除服务器{serverInfo.ServerName}吗?",
                "删除提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            ServerList.Remove(serverInfo);
            try
            {
                await ServerService.SaveGameServers(ServerList);
            }
            catch (Exception e)
            {
                _mainWindowViewModel.ShowErrorMessage("保存配置文件失败", e.Message);
                return;
            }

            _mainWindowViewModel.ShowErrorMessage("操作成功", "删除服务器成功");
        }
    }
}