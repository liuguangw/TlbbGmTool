using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.Services;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class EditPetViewModel : Pet
    {
        #region Fields

        private MainWindowViewModel _mainWindowViewModel;
        private Pet _petInfo;
        private EditPetWindow _editPetWindow;

        #endregion

        public AppCommand SavePetCommand { get; }

        public string WindowTitle => $"修改 {PetName} (ID: {PetGuid})";

        /// <summary>
        /// 性格选择
        /// </summary>
        public List<ComboBoxNode<int>> AiTypeSelection { get; }

        public EditPetViewModel()
        {
            SavePetCommand = new AppCommand(SavePet);
            AiTypeSelection = new List<ComboBoxNode<int>>
            {
                new ComboBoxNode<int> {Title = "胆小", Value = 0},
                new ComboBoxNode<int> {Title = "谨慎", Value = 1},
                new ComboBoxNode<int> {Title = "忠诚", Value = 2},
                new ComboBoxNode<int> {Title = "精明", Value = 3},
                new ComboBoxNode<int> {Title = "勇猛", Value = 4},
            };
        }

        public void InitData(MainWindowViewModel mainWindowViewModel, Pet petInfo,
            EditPetWindow editPetWindow)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _petInfo = petInfo;
            _editPetWindow = editPetWindow;
            PetGuid = petInfo.PetGuid;
            Charguid = petInfo.Charguid;
            PetName = petInfo.PetName;
            Level = petInfo.Level;
            NeedLevel = petInfo.NeedLevel;
            AiType = petInfo.AiType;
            Life = petInfo.Life;
            PetType = petInfo.PetType;
            Genera = petInfo.Genera;
            Enjoy = petInfo.Enjoy;
            //
            Strper = petInfo.Strper;
            Conper = petInfo.Conper;
            Dexper = petInfo.Dexper;
            Sprper = petInfo.Sprper;
            Iprper = petInfo.Iprper;
            //
            Savvy = petInfo.Savvy;
            Gengu = petInfo.Gengu;
            Growrate = petInfo.Growrate;
            Repoint = petInfo.Repoint;
            Exp = petInfo.Exp;
            //
            Str = petInfo.Str;
            Con = petInfo.Con;
            Dex = petInfo.Dex;
            Spr = petInfo.Spr;
            Ipr = petInfo.Ipr;
            Skill = petInfo.Skill;
        }

        private async void SavePet()
        {
            try
            {
                await DoSavePet();
            }
            catch (Exception e)
            {
                _mainWindowViewModel.ShowErrorMessage("保存失败", e.Message);
                return;
            }

            //更新属性
            _petInfo.PetName = PetName;
            _petInfo.Level = Level;
            _petInfo.NeedLevel = NeedLevel;
            _petInfo.AiType = AiType;
            _petInfo.Life = Life;
            _petInfo.PetType = PetType;
            _petInfo.Genera = Genera;
            _petInfo.Enjoy = Enjoy;
            //
            _petInfo.Strper = Strper;
            _petInfo.Conper = Conper;
            _petInfo.Dexper = Dexper;
            _petInfo.Sprper = Sprper;
            _petInfo.Iprper = Iprper;
            //
            _petInfo.Savvy = Savvy;
            _petInfo.Gengu = Gengu;
            _petInfo.Growrate = Growrate;
            _petInfo.Repoint = Repoint;
            _petInfo.Exp = Exp;
            //
            _petInfo.Str = Str;
            _petInfo.Con = Con;
            _petInfo.Dex = Dex;
            _petInfo.Spr = Spr;
            _petInfo.Ipr = Ipr;
            //更新标题
            RaisePropertyChanged(nameof(WindowTitle));
            _mainWindowViewModel.ShowSuccessMessage("保存成功", "保存珍兽信息成功");
            _editPetWindow.Close();
        }

        /// <summary>
        /// save
        /// </summary>
        /// <returns></returns>
        private async Task DoSavePet()
        {
            var sql = "UPDATE t_pet SET";
            var intDictionary = new Dictionary<string, int>()
            {
                ["level"] = Level,
                ["needlevel"] = NeedLevel,
                ["aitype"] = AiType,
                ["life"] = Life,
                ["pettype"] = PetType,
                ["genera"] = Genera,
                ["enjoy"] = Enjoy,
                //
                ["strper"] = Strper,
                ["conper"] = Conper,
                ["dexper"] = Dexper,
                ["sprper"] = Sprper,
                ["iprper"] = Iprper,
                //
                ["savvy"] = Savvy,
                ["gengu"] = Gengu,
                ["growrate"] = Growrate,
                ["repoint"] = Repoint,
                ["exp"] = Exp,
                //
                ["str"] = Str,
                ["con"] = Con,
                ["dex"] = Dex,
                ["spr"] = Spr,
                ["ipr"] = Ipr,
                //
                ["charguid"] = Charguid,
                ["lpetguid"] = PetGuid
            };
            var fieldNames = intDictionary.Keys.ToList();
            fieldNames.Add("petname");
            var updateCondition = (from fieldName in fieldNames
                select $"{fieldName}=@{fieldName}");
            sql += " " + string.Join(", ", updateCondition) + " WHERE charguid=@charguid AND lpetguid=@lpetguid";
            //构造参数
            var mySqlParameters = (from intParameter in intDictionary
                select new MySqlParameter("@" + intParameter.Key, MySqlDbType.Int32)
                {
                    Value = intParameter.Value
                }).ToList();
            mySqlParameters.Add(new MySqlParameter("@petname", MySqlDbType.String)
            {
                Value = DbStringService.ToDbString(PetName)
            });
            //
            var mySqlConnection = _mainWindowViewModel.MySqlConnection;
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            mySqlParameters.ForEach(mySqlParameter => mySqlCommand.Parameters.Add(mySqlParameter));
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