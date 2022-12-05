using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.Services;

namespace TlbbGmTool.ViewModels
{
    public class EditRoleViewModel : GameRole
    {
        #region Fields

        private EditRoleWindowViewModel _editRoleWindowViewModel;
        private MainWindowViewModel _mainWindowViewModel;
        private GameRole _gameRole;

        #endregion

        public AppCommand SaveRoleCommand { get; }
        public AppCommand GoHomeCommand { get; }
        public List<ComboBoxNode<int>> MenpaiSelection { get; private set; } = new List<ComboBoxNode<int>>();

        public EditRoleViewModel()
        {
            SaveRoleCommand = new AppCommand(SaveRole);
            GoHomeCommand = new AppCommand(GoHome);
        }

        public void InitData(EditRoleWindowViewModel editRoleWindowViewModel)
        {
            _editRoleWindowViewModel = editRoleWindowViewModel;
            _mainWindowViewModel = editRoleWindowViewModel.MainWindowViewModel;
            _gameRole = editRoleWindowViewModel.GameRole;
            if (MenpaiSelection.Count == 0)
            {
                MenpaiSelection =
                    MenpaiList.Select(
                        menpaiPair
                            =>
                            new ComboBoxNode<int>
                            {
                                Title = menpaiPair.Value,
                                Value = menpaiPair.Key
                            }
                    ).ToList();
                RaisePropertyChanged(nameof(MenpaiSelection));
            }

            //初始化属性
            Accname = _gameRole.Accname;
            Charguid = _gameRole.Charguid;
            Charname = _gameRole.Charname;
            Title = _gameRole.Title;
            Menpai = _gameRole.Menpai;
            Level = _gameRole.Level;
            Scene = _gameRole.Scene;
            Xpos = _gameRole.Xpos;
            Zpos = _gameRole.Zpos;
            Hp = _gameRole.Hp;
            Mp = _gameRole.Mp;
            Str = _gameRole.Str;
            Spr = _gameRole.Spr;
            Con = _gameRole.Con;
            Ipr = _gameRole.Ipr;
            Dex = _gameRole.Dex;
            Points = _gameRole.Points;
            Enegry = _gameRole.Enegry;
            Energymax = _gameRole.Energymax;
            Vigor = _gameRole.Vigor;
            Maxvigor = _gameRole.Maxvigor;
            Exp = _gameRole.Exp;
            Pkvalue = _gameRole.Pkvalue;
            Vmoney = _gameRole.Vmoney;
            Bankmoney = _gameRole.Bankmoney;
            Yuanbao = _gameRole.Yuanbao;
            Menpaipoint = _gameRole.Menpaipoint;
            Zengdian = _gameRole.Zengdian;
        }

        private async void SaveRole()
        {
            try
            {
                await DoSaveRole();
            }
            catch (Exception e)
            {
                _mainWindowViewModel.ShowErrorMessage("保存角色失败", e.Message);
                return;
            }

            //模型数据更新
            _gameRole.Accname = Accname;
            _gameRole.Charname = Charname;
            _gameRole.Title = Title;
            _gameRole.Menpai = Menpai;
            _gameRole.Level = Level;
            _gameRole.Scene = Scene;
            _gameRole.Xpos = Xpos;
            _gameRole.Zpos = Zpos;
            _gameRole.Hp = Hp;
            _gameRole.Mp = Mp;
            _gameRole.Str = Str;
            _gameRole.Spr = Spr;
            _gameRole.Con = Con;
            _gameRole.Ipr = Ipr;
            _gameRole.Dex = Dex;
            _gameRole.Points = Points;
            _gameRole.Enegry = Enegry;
            _gameRole.Energymax = Energymax;
            _gameRole.Vigor = Vigor;
            _gameRole.Maxvigor = Maxvigor;
            _gameRole.Exp = Exp;
            _gameRole.Pkvalue = Pkvalue;
            _gameRole.Vmoney = Vmoney;
            _gameRole.Bankmoney = Bankmoney;
            _gameRole.Yuanbao = Yuanbao;
            _gameRole.Menpaipoint = Menpaipoint;
            _gameRole.Zengdian = Zengdian;
            _editRoleWindowViewModel.NotifyWindowTitleChange();
            _mainWindowViewModel.ShowSuccessMessage("保存成功", "保存角色信息成功");
        }

        private async Task DoSaveRole()
        {
            var sql = "UPDATE t_char SET";
            //int类型的字段
            var intDictionary = new Dictionary<string, int>()
            {
                ["charguid"] = Charguid,
                ["menpai"] = Menpai,
                ["level"] = Level,
                ["scene"] = Scene,
                ["xpos"] = Xpos,
                ["zpos"] = Zpos,
                ["hp"] = Hp,
                ["mp"] = Mp,
                ["str"] = Str,
                ["spr"] = Spr,
                ["con"] = Con,
                ["ipr"] = Ipr,
                ["dex"] = Dex,
                ["points"] = Points,
                ["enegry"] = Enegry,
                ["energymax"] = Energymax,
                ["vigor"] = Vigor,
                ["maxvigor"] = Maxvigor,
                ["exp"] = Exp,
                ["pkvalue"] = Pkvalue,
                ["vmoney"] = Vmoney,
                ["bankmoney"] = Bankmoney,
                ["yuanbao"] = Yuanbao,
                ["menpaipoint"] = Menpaipoint,
                ["zengdian"] = Zengdian
            };
            var fieldNames = intDictionary.Keys.ToList();
            fieldNames.AddRange(new[] {"accname", "charname", "title"});
            // fieldA=@fieldA
            var updateCondition = (from fieldName in fieldNames
                select $"{fieldName}=@{fieldName}");
            sql += " " + string.Join(", ", updateCondition) + " WHERE charguid=@charguid";
            //构造参数
            var mySqlParameters = (from intParameter in intDictionary
                select new MySqlParameter("@" + intParameter.Key, MySqlDbType.Int32)
                {
                    Value = intParameter.Value
                }).ToList();
            mySqlParameters.Add(new MySqlParameter("@accname", MySqlDbType.String)
            {
                Value = DbStringService.ToDbString(Accname)
            });
            mySqlParameters.Add(new MySqlParameter("@charname", MySqlDbType.String)
            {
                Value = DbStringService.ToDbString(Charname)
            });
            mySqlParameters.Add(new MySqlParameter("@title", MySqlDbType.String)
            {
                Value = DbStringService.ToDbString(Title)
            });
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

        private void GoHome()
        {
            Scene = 2;
            Xpos = 160 * 100;
            Zpos = 149 * 100;
        }
    }
}