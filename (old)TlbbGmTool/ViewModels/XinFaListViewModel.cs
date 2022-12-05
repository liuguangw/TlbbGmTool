using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class XinFaListViewModel : BindDataBase
    {
        #region Fields

        private MainWindowViewModel _mainWindowViewModel;
        private int _charguid;
        private EditRoleWindow _editRoleWindow;
        private bool _xinFaListLoaded;

        #endregion

        #region Properties

        public ObservableCollection<XinFa> XinFaList { get; } =
            new ObservableCollection<XinFa>();

        public AppCommand EditXinFaCommand { get; }

        #endregion

        public XinFaListViewModel()
        {
            EditXinFaCommand = new AppCommand(ShowEditXinFaDialog);
        }

        public void InitData(MainWindowViewModel mainWindowViewModel, int charguid, EditRoleWindow editRoleWindow)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _editRoleWindow = editRoleWindow;
            _charguid = charguid;
            if (_xinFaListLoaded)
            {
                return;
            }

            LoadXinFaList();
            _xinFaListLoaded = true;
        }

        private async void LoadXinFaList()
        {
            if (_mainWindowViewModel.ConnectionStatus != DatabaseConnectionStatus.Connected)
            {
                _mainWindowViewModel.ShowErrorMessage("出错了", "数据库未连接");
                return;
            }

            try
            {
                var xinFaList = await DoLoadXinFaList();
                foreach (var xinFaInfo in xinFaList)
                {
                    XinFaList.Add(xinFaInfo);
                }

                EditXinFaCommand.RaiseCanExecuteChanged();
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

        private void ShowEditXinFaDialog(object parameter)
        {
            var xinFaInfo = parameter as XinFa;
            var editXinFaWindow = new EditXinFaWindow(_mainWindowViewModel, xinFaInfo)
            {
                Owner = _editRoleWindow
            };
            editXinFaWindow.ShowDialog();
        }
    }
}