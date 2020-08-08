using System;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.Services;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class AddOrEditServerViewModel : GameServer
    {
        #region Fields

        private MainWindowViewModel _mainWindowViewModel;
        private AddOrEditServerWindow _addOrEditServerWindow;
        private GameServer _gameServerInfo;
        private bool _isAddServer = true;
        private bool _isConnecting;

        #endregion

        #region Properties

        public string WindowTitle => _isAddServer ? "添加服务器" : $"修改服务器 {ServerName}({DbHost})";

        public AppCommand SaveServerCommand { get; }

        public AppCommand ConnTestCommand { get; }

        #endregion

        public AddOrEditServerViewModel()
        {
            SaveServerCommand = new AppCommand(SaveServer);
            ConnTestCommand = new AppCommand(TryToConnect, () => !_isConnecting);
        }

        public void InitData(MainWindowViewModel mainWindowViewModel, GameServer gameServer,
            AddOrEditServerWindow addOrEditServerWindow)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _addOrEditServerWindow = addOrEditServerWindow;
            if (gameServer == null)
            {
                //初始化默认值
                DbPort = 3306;
                AccountDbName = "web";
                GameDbName = "tlbbdb";
                return;
            }

            _gameServerInfo = gameServer;
            _isAddServer = false;
            RaisePropertyChanged(nameof(WindowTitle));
            //初始化属性
            ServerName = gameServer.ServerName;
            DbHost = gameServer.DbHost;
            DbPort = gameServer.DbPort;
            AccountDbName = gameServer.AccountDbName;
            GameDbName = gameServer.GameDbName;
            DbUser = gameServer.DbUser;
            DbPassword = gameServer.DbPassword;
        }

        private async void SaveServer()
        {
            try
            {
                await DoSaveServer();
            }
            catch (Exception e)
            {
                _mainWindowViewModel.ShowErrorMessage("保存失败", e.Message);
                return;
            }

            var gameServerInfo = _gameServerInfo ?? new GameServer();
            //更新属性
            gameServerInfo.ServerName = ServerName;
            gameServerInfo.DbHost = DbHost;
            gameServerInfo.DbPort = DbPort;
            gameServerInfo.AccountDbName = AccountDbName;
            gameServerInfo.GameDbName = GameDbName;
            gameServerInfo.DbUser = DbUser;
            gameServerInfo.DbPassword = DbPassword;
            RaisePropertyChanged(nameof(WindowTitle));
            if (_isAddServer)
            {
                //添加到list
                _mainWindowViewModel.ServerList.Add(gameServerInfo);
            }

            _mainWindowViewModel.ShowSuccessMessage("保存成功", (_isAddServer ? "添加" : "修改") + "server成功");
            _addOrEditServerWindow.Close();
        }

        private async Task DoSaveServer()
        {
            var serverList = _mainWindowViewModel.ServerList.ToList();
            var serverInfo = new GameServer()
            {
                ServerName = ServerName,
                DbHost = DbHost,
                DbPort = DbPort,
                AccountDbName = AccountDbName,
                GameDbName = GameDbName,
                DbUser = DbUser,
                DbPassword = DbPassword
            };
            if (_isAddServer)
            {
                serverList.Add(serverInfo);
            }
            else
            {
                //查找并替换对应的server,不可直接修改原对象
                for (var i = 0; i < serverList.Count; i++)
                {
                    var serverItem = serverList[i];
                    if (serverItem != _gameServerInfo)
                    {
                        continue;
                    }

                    serverList[i] = serverInfo;
                    break;
                }
            }

            await ServerService.SaveGameServers(serverList);
        }

        private async void TryToConnect()
        {
            var connectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = DbHost,
                Port = DbPort,
                Database = AccountDbName,
                UserID = DbUser,
                Password = DbPassword
            };

            var mySqlConnection = new MySqlConnection
            {
                ConnectionString = connectionStringBuilder.GetConnectionString(true),
            };
            _isConnecting = true;
            ConnTestCommand.RaiseCanExecuteChanged();
            try
            {
                await Task.Run(async () =>
                {
                    await mySqlConnection.OpenAsync();
                    await mySqlConnection.CloseAsync();
                });
                _isConnecting = false;
                ConnTestCommand.RaiseCanExecuteChanged();
            }
            catch (Exception e)
            {
                _isConnecting = false;
                ConnTestCommand.RaiseCanExecuteChanged();
                _mainWindowViewModel.ShowErrorMessage("连接失败", e.Message);
                return;
            }

            _mainWindowViewModel.ShowSuccessMessage("连接成功", "连接成功");
        }
    }
}