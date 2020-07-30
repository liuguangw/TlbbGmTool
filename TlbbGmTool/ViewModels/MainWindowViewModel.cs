using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.Services;

namespace TlbbGmTool.ViewModels
{
    public class MainWindowViewModel : BindDataBase
    {
        #region Fields

        private GameServer _selectedServer;

        private DatabaseConnectionStatus _connectionStatus = DatabaseConnectionStatus.NoConnection;

        /// <summary>
        /// MySQL连接
        /// </summary>
        private MySqlConnection _mySqlConnection;

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

        public DatabaseConnectionStatus ConnectionStatus
        {
            get => _connectionStatus;
            private set
            {
                if (!SetProperty(ref _connectionStatus, value))
                {
                    return;
                }

                RaisePropertyChanged(nameof(CanSelectServer));
                RaisePropertyChanged(nameof(CanDisconnectServer));
                ConnectCommand?.RaiseCanExecuteChanged();
                DisconnectCommand?.RaiseCanExecuteChanged();
            }
        }

        public bool CanSelectServer =>
            ServerList.Count > 0 && _connectionStatus == DatabaseConnectionStatus.NoConnection;

        public bool CanDisconnectServer => _connectionStatus == DatabaseConnectionStatus.Connected;

        /// <summary>
        /// 连接命令
        /// </summary>
        public AppCommand ConnectCommand { get; }

        /// <summary>
        /// 断开命令
        /// </summary>
        public AppCommand DisconnectCommand { get; }

        public MySqlConnection MySqlConnection
        {
            get => _mySqlConnection;
            private set
            {
                if (SetProperty(ref _mySqlConnection, value))
                {
                    ConnectionStatus = value == null
                        ? DatabaseConnectionStatus.NoConnection
                        : DatabaseConnectionStatus.Connected;
                }
            }
        }

        public Dictionary<int, PetSkill> PetSkills { get; private set; } = new Dictionary<int, PetSkill>();

        public Dictionary<int, CommonItem> CommonItems { get; private set; } = new Dictionary<int, CommonItem>();

        #endregion

        public MainWindowViewModel()
        {
            ConnectCommand = new AppCommand(ConnectServer,
                () => CanSelectServer);
            DisconnectCommand = new AppCommand(DisconnectServer,
                () => CanDisconnectServer);
            ServerList.CollectionChanged += (sender, e)
                =>
            {
                RaisePropertyChanged(nameof(CanSelectServer));
                ConnectCommand.RaiseCanExecuteChanged();
            };
        }

        public async Task LoadApplicationData()
        {
            var servers = await ServerService.LoadGameServers();
            foreach (var server in servers)
            {
                ServerList.Add(server);
            }

            if (ServerList.Count > 0)
            {
                SelectedServer = ServerList.First();
            }

            var loadPetSkillListTask = TextFileService.LoadPetSkillList();
            var loadCommonItemsTask = TextFileService.LoadCommonItemList();
            //等待配置文件读取完成
            await Task.WhenAll(loadPetSkillListTask, loadCommonItemsTask);
            PetSkills = loadPetSkillListTask.Result;
            CommonItems = loadCommonItemsTask.Result;
        }

        public void ShowErrorMessage(string title, string content) =>
            MessageBox.Show(content, title, MessageBoxButton.OK, MessageBoxImage.Error);

        public void ShowSuccessMessage(string title, string content) =>
            MessageBox.Show(content, title, MessageBoxButton.OK, MessageBoxImage.Information);

        private async void ConnectServer()
        {
            if (SelectedServer == null)
            {
                return;
            }

            var connectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = _selectedServer.DbHost,
                Port = _selectedServer.DbPort,
                Database = _selectedServer.AccountDbName,
                UserID = _selectedServer.DbUser,
                Password = _selectedServer.DbPassword
            };

            var mySqlConnection = new MySqlConnection
            {
                ConnectionString = connectionStringBuilder.GetConnectionString(true),
            };
            try
            {
                ConnectionStatus = DatabaseConnectionStatus.Pending;
                await Task.Run(async () => await mySqlConnection.OpenAsync());
            }
            catch (Exception e)
            {
                ConnectionStatus = DatabaseConnectionStatus.NoConnection;
                ShowErrorMessage("连接数据库出错", e.Message);
                return;
            }

            MySqlConnection = mySqlConnection;
            SelectedServer.Connected = true;
        }

        private async void DisconnectServer()
        {
            if (SelectedServer == null)
            {
                return;
            }

            try
            {
                ConnectionStatus = DatabaseConnectionStatus.Pending;
                await _mySqlConnection.CloseAsync();
            }
            catch (Exception e)
            {
                ShowErrorMessage("断开连接出错", e.Message);
            }

            MySqlConnection = null;
            SelectedServer.Connected = false;
        }
    }
}