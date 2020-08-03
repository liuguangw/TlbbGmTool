using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.Services;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class RoleListViewModel : BindDataBase
    {
        #region Fields

        private string _roleSearchText = string.Empty;
        private string _accountSearchText = string.Empty;
        private MainWindowViewModel _mainWindowViewModel;
        private MainWindow _mainWindow;

        #endregion


        /// <summary>
        /// account list
        /// </summary>
        public ObservableCollection<GameRole> RoleList { get; } =
            new ObservableCollection<GameRole>();

        #region Properties

        /// <summary>
        /// 搜索昵称的文本
        /// </summary>
        public string RoleSearchText
        {
            get => _roleSearchText;
            set => SetProperty(ref _roleSearchText, value);
        }

        /// <summary>
        /// 搜索账号的文本
        /// </summary>
        public string AccountSearchText
        {
            get => _accountSearchText;
            set => SetProperty(ref _accountSearchText, value);
        }

        /// <summary>
        /// 搜索命令
        /// </summary>
        public AppCommand SearchCommand { get; }

        public AppCommand EditRoleCommand { get; }

        #endregion

        public RoleListViewModel()
        {
            SearchCommand = new AppCommand(SearchRole);
            EditRoleCommand = new AppCommand(ShowEditRoleDialog);
        }


        public void InitData(MainWindowViewModel mainWindowViewModel, MainWindow mainWindow)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _mainWindow = mainWindow;
        }

        private async void SearchRole()
        {
            if (_mainWindowViewModel.ConnectionStatus != DatabaseConnectionStatus.Connected)
            {
                _mainWindowViewModel.ShowErrorMessage("出错了", "数据库未连接");
                return;
            }

            RoleList.Clear();
            try
            {
                var roleList = await DoSearchRole();
                foreach (var roleInfo in roleList)
                {
                    RoleList.Add(roleInfo);
                }
            }
            catch (Exception e)
            {
                _mainWindowViewModel.ShowErrorMessage("搜索出错", e.Message);
            }
        }

        private async Task<List<GameRole>> DoSearchRole()
        {
            var roleList = new List<GameRole>();
            var mySqlConnection = _mainWindowViewModel.MySqlConnection;
            //构造SQL语句
            var sql = "SELECT * FROM t_char";
            var searchDictionary = new Dictionary<string, string>();
            if (_roleSearchText != string.Empty || _accountSearchText != string.Empty)
            {
                var conditionStr = string.Empty;
                if (_roleSearchText != string.Empty)
                {
                    searchDictionary["charname"] = DbStringService.ToDbString(_roleSearchText);
                    conditionStr = "charname LIKE @charname";
                }

                if (_accountSearchText != string.Empty)
                {
                    searchDictionary["accname"] = _accountSearchText;
                    if (conditionStr != string.Empty)
                    {
                        conditionStr += " AND ";
                    }

                    conditionStr += "accname LIKE @accname";
                }

                sql += " WHERE " + conditionStr;
            }

            sql += " ORDER BY aid ASC LIMIT 50";
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            var mySqlParameters = (from searchInfo in searchDictionary
                select new MySqlParameter($"@{searchInfo.Key}", MySqlDbType.String)
                {
                    Value = $"%{searchInfo.Value}%"
                }).ToList();
            mySqlParameters.ForEach(mySqlParameter => mySqlCommand.Parameters.Add(mySqlParameter));
            await Task.Run(async () =>
            {
                var gameDbName = _mainWindowViewModel.SelectedServer.GameDbName;
                if (mySqlConnection.Database != gameDbName)
                {
                    // 切换数据库
                    await mySqlConnection.ChangeDataBaseAsync(gameDbName);
                }

                using (var rd = await mySqlCommand.ExecuteReaderAsync() as MySqlDataReader)
                {
                    while (await rd.ReadAsync())
                    {
                        var gameRole = new GameRole()
                        {
                            Accname = rd.GetString("accname"),
                            Charguid = rd.GetInt32("charguid"),
                            Charname = DbStringService.ToCommonString(rd.GetString("charname")),
                            Title = DbStringService.ToCommonString(rd.GetString("title")),
                            Menpai = rd.GetInt32("menpai"),
                            Level = rd.GetInt32("level"),
                            Scene = rd.GetInt32("scene"),
                            Xpos = rd.GetInt32("xpos"),
                            Zpos = rd.GetInt32("zpos"),
                            Hp = rd.GetInt32("hp"),
                            Mp = rd.GetInt32("mp"),
                            Str = rd.GetInt32("str"),
                            Spr = rd.GetInt32("spr"),
                            Con = rd.GetInt32("con"),
                            Ipr = rd.GetInt32("ipr"),
                            Dex = rd.GetInt32("dex"),
                            Points = rd.GetInt32("points"),
                            Enegry = rd.GetInt32("enegry"),
                            Energymax = rd.GetInt32("energymax"),
                            Vigor = rd.GetInt32("vigor"),
                            Maxvigor = rd.GetInt32("maxvigor"),
                            Exp = rd.GetInt32("exp"),
                            Pkvalue = rd.GetInt32("pkvalue"),
                            Vmoney = rd.GetInt32("vmoney"),
                            Bankmoney = rd.GetInt32("bankmoney"),
                            Yuanbao = rd.GetInt32("yuanbao"),
                            Menpaipoint = rd.GetInt32("menpaipoint"),
                            Zengdian = rd.GetInt32("zengdian"),
                        };
                        //add to list
                        roleList.Add(gameRole);
                    }
                }
            });
            return roleList;
        }

        private void ShowEditRoleDialog(object parameter)
        {
            var gameRole = parameter as GameRole;
            var editRoleWindow = new EditRoleWindow(_mainWindowViewModel, gameRole)
            {
                Owner = _mainWindow
            };
            editRoleWindow.ShowDialog();
        }
    }
}