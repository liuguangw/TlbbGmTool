using System;
using System.Collections.ObjectModel;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.Services;

namespace TlbbGmTool.ViewModels
{
    public class ServerListViewModel : BindDataBase
    {
        #region Properties

        public ObservableCollection<GameServer> ServerList { get; private set; }
            = new ObservableCollection<GameServer>();

        public MainWindowViewModel MainWindowViewModel { get; private set; }

        #endregion

        public void InitData(MainWindowViewModel mainWindowViewModel)
        {
            MainWindowViewModel = mainWindowViewModel;
            ServerList = mainWindowViewModel.ServerList;
            RaisePropertyChanged(nameof(ServerList));
        }

        public async void ProcessDeleteServer(GameServer serverInfo)
        {
            ServerList.Remove(serverInfo);
            try
            {
                await ServerService.SaveGameServers(ServerList);
            }
            catch (Exception e)
            {
                MainWindowViewModel.ShowErrorMessage("保存配置文件失败", e.Message);
                return;
            }

            MainWindowViewModel.ShowErrorMessage("操作成功", "删除服务器成功");
        }
    }
}