using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.Services;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class PetListViewModel : BindDataBase
    {
        #region Fields

        private MainWindowViewModel _mainWindowViewModel;
        private EditRoleWindow _editRoleWindow;
        private int _charguid;
        private bool _petListLoaded;

        #endregion

        #region Properties

        public ObservableCollection<Pet> PetList { get; } =
            new ObservableCollection<Pet>();

        public AppCommand EditPetCommand { get; }

        public AppCommand DeletePetCommand { get; }

        public AppCommand EditPetSkillCommand { get; }

        #endregion

        public PetListViewModel()
        {
            EditPetCommand = new AppCommand(ShowEditPetDialog);
            EditPetSkillCommand = new AppCommand(ShowEditPetSkillDialog);
            DeletePetCommand = new AppCommand(ProcessDelete);
        }

        public void InitData(MainWindowViewModel mainWindowViewModel, int charguid, EditRoleWindow editRoleWindow)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _editRoleWindow = editRoleWindow;
            _charguid = charguid;

            if (_petListLoaded)
            {
                return;
            }

            LoadPetList();
            _petListLoaded = true;
        }

        private async void LoadPetList()
        {
            if (_mainWindowViewModel.ConnectionStatus != DatabaseConnectionStatus.Connected)
            {
                _mainWindowViewModel.ShowErrorMessage("出错了", "数据库未连接");
                return;
            }

            try
            {
                var itemList = await DoLoadPetList();
                foreach (var item in itemList)
                {
                    PetList.Add(item);
                }

                EditPetCommand.RaiseCanExecuteChanged();
                EditPetSkillCommand.RaiseCanExecuteChanged();
            }
            catch (Exception e)
            {
                _mainWindowViewModel.ShowErrorMessage("加载出错", e.Message);
            }
        }

        private async Task<List<Pet>> DoLoadPetList()
        {
            var itemList = new List<Pet>();
            var mySqlConnection = _mainWindowViewModel.MySqlConnection;
            var sql = "SELECT * FROM t_pet WHERE charguid=" + _charguid
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
                        var itemInfo = new Pet()
                        {
                            PetGuid = rd.GetInt32("lpetguid"),
                            Charguid = rd.GetInt32("charguid"),
                            PetName = DbStringService.ToCommonString(rd.GetString("petname")),
                            Level = rd.GetInt32("level"),
                            NeedLevel = rd.GetInt32("needlevel"),
                            AiType = rd.GetInt32("aitype"),
                            Life = rd.GetInt32("life"),
                            PetType = rd.GetInt32("pettype"),
                            Genera = rd.GetInt32("genera"),
                            Enjoy = rd.GetInt32("enjoy"),
                            Strper = rd.GetInt32("strper"),
                            Conper = rd.GetInt32("conper"),
                            Dexper = rd.GetInt32("dexper"),
                            Sprper = rd.GetInt32("sprper"),
                            Iprper = rd.GetInt32("iprper"),
                            Savvy = rd.GetInt32("savvy"),
                            Gengu = rd.GetInt32("gengu"),
                            Growrate = rd.GetInt32("growrate"),
                            Repoint = rd.GetInt32("repoint"),
                            Exp = rd.GetInt32("exp"),
                            Str = rd.GetInt32("str"),
                            Con = rd.GetInt32("con"),
                            Dex = rd.GetInt32("dex"),
                            Spr = rd.GetInt32("spr"),
                            Ipr = rd.GetInt32("ipr"),
                            Skill = rd.GetString("skill") ?? string.Empty
                        };
                        itemList.Add(itemInfo);
                    }
                }
            });
            return itemList;
        }

        private void ShowEditPetDialog(object parameter)
        {
            var petInfo = parameter as Pet;
            var editPetWindow = new EditPetWindow(_mainWindowViewModel, petInfo)
            {
                Owner = _editRoleWindow
            };
            editPetWindow.ShowDialog();
        }

        private void ShowEditPetSkillDialog(object parameter)
        {
            var petInfo = parameter as Pet;
            var editPetSkillWindow = new EditPetSkillWindow(_mainWindowViewModel, petInfo)
            {
                Owner = _editRoleWindow
            };
            editPetSkillWindow.ShowDialog();
        }

        private async void ProcessDelete(object parameter)
        {
            var petInfo = parameter as Pet;
            var tipName = $"{petInfo.PetName}(ID={petInfo.PetGuid})";
            //删除确认
            if (MessageBox.Show(_editRoleWindow, $"确定要删除 {tipName}吗?",
                "删除提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                await DoProcessDelete(petInfo.Charguid, petInfo.PetGuid);
            }
            catch (Exception e)
            {
                _mainWindowViewModel.ShowErrorMessage("删除失败", e.Message);
                return;
            }

            //删除成功后,将petInfo从列表移出
            PetList.Remove(petInfo);
            _mainWindowViewModel.ShowSuccessMessage("删除成功",
                $"删除 {tipName}成功");
        }

        private async Task DoProcessDelete(int charguid, int petGuid)
        {
            var sql = $"DELETE FROM t_pet WHERE charguid={charguid} AND lpetguid={petGuid}";
            var mySqlConnection = _mainWindowViewModel.MySqlConnection;
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