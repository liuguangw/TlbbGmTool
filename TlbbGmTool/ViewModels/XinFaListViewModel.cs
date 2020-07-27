using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TlbbGmTool.Core;
using TlbbGmTool.Models;

namespace TlbbGmTool.ViewModels
{
    public class XinFaListViewModel : BindDataBase
    {
        #region Fields

        private MainWindowViewModel _mainWindowViewModel;
        private int _charguid;

        #endregion

        #region Properties

        public ObservableCollection<XinFa> XinFaList { get; } =
            new ObservableCollection<XinFa>();

        #endregion

        public void InitData(EditRoleWindowViewModel editRoleWindowViewModel)
        {
            if (_mainWindowViewModel == null)
            {
                _mainWindowViewModel = editRoleWindowViewModel.MainWindowViewModel;
                _charguid = editRoleWindowViewModel.GameRole.Charguid;
            }
            // 每次切换到此页面时都重新加载
            LoadXinFaList();
        }

        private async void LoadXinFaList()
        {
            if (_mainWindowViewModel.ConnectionStatus != DatabaseConnectionStatus.Connected)
            {
                _mainWindowViewModel.ShowErrorMessage("出错了", "数据库未连接");
                return;
            }

            XinFaList.Clear();
            try
            {
                var xinFaList = await DoLoadXinFaList();
                foreach (var xinFaInfo in xinFaList)
                {
                    XinFaList.Add(xinFaInfo);
                }
            }
            catch (Exception e)
            {
                _mainWindowViewModel.ShowErrorMessage("加载出错", e.Message);
            }
        }

        private async Task<List<XinFa>> DoLoadXinFaList()
        {
            var xinFaList = new List<XinFa>();
            var mySqlConnection = _mainWindowViewModel.MySqlConnection;
            var sql = "SELECT * FROM t_xinfa WHERE charguid=" + _charguid
                                                              + " ORDER BY aid ASC";
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
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
                        var xinFaInfo = new XinFa()
                        {
                            Aid = rd.GetInt32("aid"),
                            Charguid = rd.GetInt32("charguid"),
                            Xinfaid = rd.GetInt32("xinfaid"),
                            Xinfalvl = rd.GetInt32("xinfalvl")
                        };
                        xinFaList.Add(xinFaInfo);
                    }
                }
            });
            return xinFaList;
        }
    }
}