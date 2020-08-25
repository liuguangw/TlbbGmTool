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

        private bool _allDataLoaded;
        private string _dbVersion;

        #endregion

        #region Properties

        public string WindowTitle
        {
            get
            {
                var title = "天龙八部GM工具 - by 流光";
                if (!_allDataLoaded)
                {
                    title += "(加载配置中...)";
                }

                if (!string.IsNullOrEmpty(_dbVersion))
                {
                    title += $"(MySQL: {_dbVersion})";
                }

                return title;
            }
        }

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

        public bool AllDataLoaded
        {
            get => _allDataLoaded;
            set
            {
                if (SetProperty(ref _allDataLoaded, value))
                {
                    RaisePropertyChanged(nameof(WindowTitle));
                }
            }
        }

        private string DbVersion
        {
            set
            {
                if (SetProperty(ref _dbVersion, value))
                {
                    RaisePropertyChanged(nameof(WindowTitle));
                }
            }
        }

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

        public Dictionary<int, ItemBase> ItemBases { get; private set; } = new Dictionary<int, ItemBase>();

        public Dictionary<int, string> Attr1CategoryList = new Dictionary<int, string>();
        public Dictionary<int, string> Attr2CategoryList = new Dictionary<int, string>();

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
            var loadGemItemsTask = TextFileService.LoadGemItemList();
            var loadEquipBasesTask = TextFileService.LoadEquipBaseList();
            var loadCommonConfigTask = CommonConfigService.LoadMenpaiAndAttrAsync();
            //等待配置文件读取完成
            await Task.WhenAll(loadPetSkillListTask, loadCommonItemsTask, loadGemItemsTask, loadEquipBasesTask,
                loadCommonConfigTask);
            loadPetSkillListTask.Result.ForEach(
                petSkill => PetSkills.Add(petSkill.Id, petSkill)
            );
            loadCommonItemsTask.Result.ForEach(
                itemInfo => ItemBases.Add(itemInfo.Id, itemInfo)
            );
            loadGemItemsTask.Result.ForEach(
                itemInfo => ItemBases.Add(itemInfo.Id, itemInfo)
            );
            loadEquipBasesTask.Result.ForEach(
                itemInfo => ItemBases.Add(itemInfo.Id, itemInfo)
            );
            var (menpaiDictionary, attr1Dictionary, attr2Dictionary) = loadCommonConfigTask.Result;
            GameRole.MenpaiList = menpaiDictionary;
            Attr1CategoryList = attr1Dictionary;
            Attr2CategoryList = attr2Dictionary;
            AllDataLoaded = true;
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
                //获取数据库版本信息
                DbVersion = await LoadDbVersionAsync(mySqlConnection);
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

        private static async Task<string> LoadDbVersionAsync(MySqlConnection mySqlConnection)
        {
            const string sql = "SELECT version() AS v";
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            string version = null;
            await Task.Run(async () =>
            {
                var rd = await mySqlCommand.ExecuteScalarAsync();
                version = rd.ToString();
            });
            return version;
        }

        private async void DisconnectServer()
        {
            if (SelectedServer == null)
            {
                return;
            }

            DbVersion = null;
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