using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TlbbGmTool.Core;
using TlbbGmTool.Models;

namespace TlbbGmTool.ViewModels
{
    public class RoleListViewModel : BindDataBase
    {
        #region Fields

        private string _searchText = string.Empty;

        #endregion


        /// <summary>
        /// account list
        /// </summary>
        public ObservableCollection<GameRole> RoleList { get; } =
            new ObservableCollection<GameRole>();

        /// <summary>
        /// Main window ViewModel
        /// </summary>
        public MainWindowViewModel MainWindowViewModel { get; set; }

        #region Properties

        /// <summary>
        /// 搜索文本
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        /// <summary>
        /// 搜索命令
        /// </summary>
        public AppCommand SearchCommand { get; }

        #endregion

        public RoleListViewModel()
        {
            SearchCommand = new AppCommand(SearchRole);
        }

        private async void SearchRole()
        {
            if (MainWindowViewModel.ConnectionStatus != DatabaseConnectionStatus.Connected)
            {
                MainWindowViewModel.ShowErrorMessage("出错了", "数据库未连接");
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
                MainWindowViewModel.ShowErrorMessage("搜索出错", e.Message);
            }
        }

        private async Task<List<GameRole>> DoSearchRole()
        {
            var roleList = new List<GameRole>();
            var mySqlConnection = MainWindowViewModel.MySqlConnection;
            var sql = "SELECT * FROM t_char";
            if (_searchText != string.Empty)
            {
                sql += " WHERE charname like @searchText";
            }

            sql += " ORDER BY aid ASC LIMIT 50";
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            if (_searchText != string.Empty)
            {
                var searchParam = new MySqlParameter("@searchText", MySqlDbType.String)
                {
                    Value = $"%{_searchText}%"
                };
                mySqlCommand.Parameters.Add(searchParam);
            }

            await Task.Run(async () =>
            {
                var gameDbName = MainWindowViewModel.SelectedServer.GameDbName;
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
                            Aid = rd.GetInt32("aid"),
                            Accname = rd.GetString("accname"),
                            Charguid = rd.GetInt32("charguid"),
                            Charname = rd.GetString("charname"),
                            Title = rd.GetString("title"),
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
    }
}