using liuguang.TlbbGmTool.Models;
using System.Collections.Generic;

namespace liuguang.TlbbGmTool.ViewModels;
public class PetSkillViewModel
{
    private PetSkillBase _baseInfo;
    public int Id => _baseInfo.Id;

    public int SkillType => _baseInfo.SkillType;

    public string SkillTypeText
    {
        get
        {
            var typeText = string.Empty;
            switch (_baseInfo.SkillType)
            {
                case 0:
                    typeText = "手动";
                    break;
                case 1:
                    typeText = "自动";
                    break;
                case 2:
                    typeText = "buff";
                    break;
            }
            return typeText;
        }
    }

    public string Name => _baseInfo.Name;

    public string Description => _baseInfo.Description;

    public PetSkillViewModel(PetSkillBase baseInfo) { _baseInfo = baseInfo; }
}
