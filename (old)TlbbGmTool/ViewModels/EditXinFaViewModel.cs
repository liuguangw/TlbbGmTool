using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class EditXinFaViewModel : XinFa
    {
        #region Fields

        private MainWindowViewModel _mainWindowViewModel;
        private XinFa _xinFaInfo;
        private EditXinFaWindow _editXinFaWindow;

        #endregion

        public AppCommand SaveXinFaCommand { get; }

        public EditXinFaViewModel()
        {
            SaveXinFaCommand = new AppCommand(SaveXinFa);
        }

        public void InitData(MainWindowViewModel mainWindowViewModel, XinFa xinFaInfo,
            EditXinFaWindow editXinFaWindow)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _xinFaInfo = xinFaInfo;
            _editXinFaWindow = editXinFaWindow;
            //初始化属性
            Aid = xinFaInfo.Aid;
            Charguid = xinFaInfo.Charguid;
            Xinfaid = xinFaInfo.Xinfaid;
            Xinfalvl = xinFaInfo.Xinfalvl;
        }

        private async void SaveXinFa()
        {
            try
            {
                await DoSaveXinFa();
            }
            catch (Exception e)
            {
                _mainWindowViewModel.ShowErrorMessage("保存失败", e.Message);
                return;
            }

            //只能修改level
            _xinFaInfo.Xinfalvl = Xinfalvl;
            _mainWindowViewModel.ShowSuccessMessage("保存成功", "保存心法等级成功");
            _editXinFaWindow.Close();
        }

        private async Task DoSaveXinFa()
        {
            var mySqlConnection = _mainWindowViewModel.MySqlConnection;
            var sql = $"UPDATE t_xinfa SET xinfalvl={Xinfalvl} WHERE aid={Aid}";
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            await Task.Run(async () =>
            {
                var gameDbName = _mainWindowViewModel.SelectedServer.GameDbName;
                if (mySqlConnection.Database != gameDbName)
                {
                    // 切换数据库
                    await mySqlConnection.ChangeDataBaseAsync(gameDbName);
                }

                await mySqlCommand.ExecuteNonQueryAsync();
            });
        }
    }
}