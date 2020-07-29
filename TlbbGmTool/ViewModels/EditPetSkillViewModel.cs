using System;
using System.Collections.ObjectModel;
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

        #endregion

        #region Properties

        public ObservableCollection<PetSkill> SkillList { get; }
            = new ObservableCollection<PetSkill>();

        #endregion

        public void InitData(Pet petInfo, MainWindowViewModel mainWindowViewModel,
            EditPetSkillWindow editPetSkillWindow)
        {
            _petInfo = petInfo;
            _mainWindowViewModel = mainWindowViewModel;
            _editPetSkillWindow = editPetSkillWindow;
            LoadSkillList();
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
    }
}