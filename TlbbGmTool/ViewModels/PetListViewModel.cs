using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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

        #endregion

        #region Properties

        public ObservableCollection<Pet> PetList { get; } =
            new ObservableCollection<Pet>();

        public AppCommand EditPetCommand { get; }

        public AppCommand EditPetSkillCommand { get; }

        #endregion

        public PetListViewModel()
        {
            EditPetCommand = new AppCommand(ShowEditPetDialog);
            EditPetSkillCommand = new AppCommand(ShowEditPetSkillDialog);
        }

        public void InitData(MainWindowViewModel mainWindowViewModel, int charguid, EditRoleWindow editRoleWindow)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _editRoleWindow = editRoleWindow;
            _charguid = charguid;

            // 每次切换到此页面时都重新加载
            LoadPetList();
        }

        private async void LoadPetList()
        {
            if (_mainWindowViewModel.ConnectionStatus != DatabaseConnectionStatus.Connected)
            {
                _mainWindowViewModel.ShowErrorMessage("出错了", "数据库未连接");
                return;
            }

            PetList.Clear();
            try
            {
                var itemList = await DoLoadPetList();
                foreach (var item in itemList)
                {
                    PetList.Add(item);
                }
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
    }
}