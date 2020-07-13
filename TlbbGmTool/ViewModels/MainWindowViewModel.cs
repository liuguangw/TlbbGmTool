using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.Services;

namespace TlbbGmTool.ViewModels
{
    public class MainWindowViewModel : BindDataBase
    {
        #region Fields

        private GameServer _selectedServer;

        private bool _connected;

        #endregion

        #region Properties

        /// <summary>
        /// server list
        /// </summary>
        public ObservableCollection<GameServer> ServerList { get; } = new ObservableCollection<GameServer>();

        /// <summary>
        /// selected server
        /// </summary>
        public GameServer SelectedServer
        {
            get => _selectedServer;
            set => SetProperty(ref _selectedServer, value);
        }

        /// <summary>
        /// connection status
        /// </summary>
        public bool Connected
        {
            get => _connected;
            set
            {
                if (!SetProperty(ref _connected, value))
                {
                    return;
                }

                RaisePropertyChanged(nameof(CanSelectServer));
                ConnectCommand?.RaiseCanExecuteChanged();
                DisconnectCommand?.RaiseCanExecuteChanged();
            }
        }

        public bool CanSelectServer => ServerList.Count > 0 && !_connected;

        /// <summary>
        /// 连接命令
        /// </summary>
        public AppCommand ConnectCommand { get; }

        /// <summary>
        /// 断开命令
        /// </summary>
        public AppCommand DisconnectCommand { get; }

        #endregion

        public MainWindowViewModel()
        {
            ConnectCommand = new AppCommand(ConnectServer,
                () => CanSelectServer);
            DisconnectCommand = new AppCommand(DisconnectServer,
                () => Connected);
            ServerList.CollectionChanged += (sender, e)
                =>
            {
                RaisePropertyChanged(nameof(CanSelectServer));
                ConnectCommand.RaiseCanExecuteChanged();
            };
        }

        public async Task LoadApplicationData()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var configFilePath = Path.Combine(baseDir, "config", "servers.xml");
            var servers = await ServerService.LoadGameServers(configFilePath);
            foreach (var server in servers)
            {
                ServerList.Add(server);
            }

            if (ServerList.Count > 0)
            {
                SelectedServer = ServerList.First();
            }
        }

        public async void ConnectServer()
        {
            if (SelectedServer == null)
            {
                return;
            }

            SelectedServer.Connected = true;
            Connected = true;
        }

        public async void DisconnectServer()
        {
            if (SelectedServer == null)
            {
                return;
            }

            SelectedServer.Connected = false;
            Connected = false;
        }
    }
}