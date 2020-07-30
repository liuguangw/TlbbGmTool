using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class EditPetSkillViewModel : BindDataBase
    {
        #region Fields

        private Pet _petInfo;
        private MainWindowViewModel _mainWindowViewModel;
        private EditPetSkillWindow _editPetSkillWindow;
        private string _searchSkill = string.Empty;
        private PetSkill _selectedSkill;
        private int _selectedSkillType = -1;

        #endregion

        #region Properties

        public AppCommand AddPetSkillCommand { get; }

        public AppCommand DeletePetSkillCommand { get; }

        public AppCommand SavePetSkillCommand { get; }

        public string WindowTitle =>
            _petInfo == null ? string.Empty : $"修改 {_petInfo.PetName}(ID: {_petInfo.Aid}) Skill";

        public ObservableCollection<PetSkill> SkillList { get; }
            = new ObservableCollection<PetSkill>();

        public string SearchSkill
        {
            get => _searchSkill;
            set
            {
                if (!SetProperty(ref _searchSkill, value))
                {
                    return;
                }

                NotifyReloadSkillSelection();
            }
        }

        public List<PetSkill> SkillSelection
        {
            get
            {
                if (_mainWindowViewModel == null)
                {
                    return new List<PetSkill>();
                }

                var allSkills = _mainWindowViewModel.PetSkills.Values;
                return (from skillItem in allSkills
                    where skillItem.Name.IndexOf(_searchSkill) >= 0
                    where _selectedSkillType == -1 || skillItem.SkillType == _selectedSkillType
                    select skillItem).ToList();
            }
        }

        public PetSkill SelectedSkill
        {
            get => _selectedSkill;
            set => SetProperty(ref _selectedSkill, value);
        }

        public List<ComboBoxNode<int>> SkillTypeSelection { get; }

        public int SelectedSkillType
        {
            get => _selectedSkillType;
            set
            {
                if (!SetProperty(ref _selectedSkillType, value))
                {
                    return;
                }

                NotifyReloadSkillSelection();
            }
        }

        #endregion

        public EditPetSkillViewModel()
        {
            SkillTypeSelection = new List<ComboBoxNode<int>>
            {
                new ComboBoxNode<int> {Title = "全部", Value = -1},
                new ComboBoxNode<int> {Title = "主动", Value = 0},
                new ComboBoxNode<int> {Title = "被部", Value = 1},
                new ComboBoxNode<int> {Title = "buff", Value = 2}
            };
            AddPetSkillCommand = new AppCommand(AddSkillToList);
            DeletePetSkillCommand = new AppCommand(DeletePetSkill);
            SavePetSkillCommand = new AppCommand(SavePetSkill);
        }

        public void InitData(Pet petInfo, MainWindowViewModel mainWindowViewModel,
            EditPetSkillWindow editPetSkillWindow)
        {
            _petInfo = petInfo;
            _mainWindowViewModel = mainWindowViewModel;
            _editPetSkillWindow = editPetSkillWindow;
            NotifyReloadSkillSelection();
            RaisePropertyChanged(nameof(WindowTitle));
            LoadSkillList();
        }

        /// <summary>
        /// 重新加载列表
        /// </summary>
        private void NotifyReloadSkillSelection()
        {
            RaisePropertyChanged(nameof(SkillSelection));
            //默认选择第一个
            if (SkillSelection.Count > 0)
            {
                SelectedSkill = SkillSelection.First();
            }
        }

        private static PetSkill MakeUnknownSkill(int skillId)
        {
            return new PetSkill(skillId, "unknown", 1);
        }

        private void LoadSkillList()
        {
            var skillContent = _petInfo.Skill;
            const int nodeLength = 6;
            var petSkillDictionary = _mainWindowViewModel.PetSkills;
            for (var i = 0; i < 13; i++)
            {
                var offset = i * nodeLength;
                var flag = skillContent.Substring(offset, 2);
                if (flag == "00")
                {
                    continue;
                }

                var hexIdString = skillContent.Substring(offset + 4, 2) +
                                  skillContent.Substring(offset + 2, 2);
                var skillId = Convert.ToInt32(hexIdString, 16);
                SkillList.Add(petSkillDictionary.ContainsKey(skillId)
                    ? petSkillDictionary[skillId]
                    : MakeUnknownSkill(skillId));
            }
        }

        private void AddSkillToList()
        {
            if (_selectedSkill == null)
            {
                return;
            }

            //判断是否存在
            var skillExists = false;
            foreach (var skillInfo in SkillList)
            {
                if (skillInfo.Id == _selectedSkill.Id)
                {
                    skillExists = true;
                    break;
                }
            }

            const int maxSkillCount = 12;
            if (SkillList.Count >= maxSkillCount)
            {
                _mainWindowViewModel.ShowSuccessMessage("出错了", $"skill总个数不能超过{maxSkillCount}个");
                return;
            }

            if (skillExists)
            {
                _mainWindowViewModel.ShowSuccessMessage("出错了", $"skill {_selectedSkill.Name}已存在");
                return;
            }

            SkillList.Add(_selectedSkill);
        }

        private void DeletePetSkill(object parameter)
        {
            var targetSkill = parameter as PetSkill;
            SkillList.Remove(targetSkill);
        }

        private async void SavePetSkill()
        {
            var skillHexList = from skillInfo in SkillList
                let skillHexStr = skillInfo.Id.ToString("X4")
                select "01" + skillHexStr.Substring(2) + skillHexStr.Substring(0, 2);
            var skillString = string.Concat(skillHexList);
            //pad string
            skillString += string.Concat(Enumerable.Repeat("00FFFF", 13 - SkillList.Count));
            try
            {
                await DoSavePetSkill(skillString);
            }
            catch (Exception e)
            {
                _mainWindowViewModel.ShowErrorMessage("保存出错", e.Message);
                return;
            }

            _petInfo.Skill = skillString;
            _mainWindowViewModel.ShowSuccessMessage("保存成功", "保存skill信息成功");
            _editPetSkillWindow.Close();
        }

        private async Task DoSavePetSkill(string skillString)
        {
            const string sql = "UPDATE t_pet SET skill=@skill WHERE aid=@aid";
            var mySqlConnection = _mainWindowViewModel.MySqlConnection;
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            mySqlCommand.Parameters.Add(new MySqlParameter("@skill", MySqlDbType.String)
            {
                Value = skillString
            });
            mySqlCommand.Parameters.Add(new MySqlParameter("@aid", MySqlDbType.Int32)
            {
                Value = _petInfo.Aid
            });
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