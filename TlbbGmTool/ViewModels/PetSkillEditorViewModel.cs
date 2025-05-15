using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Services;
using liuguang.TlbbGmTool.ViewModels.Data;

namespace liuguang.TlbbGmTool.ViewModels;
public class PetSkillEditorViewModel : ViewModelBase
{
    #region Fields
    private bool _isSaving = false;
    private PetLogViewModel? _inputPetInfo;
    private PetLogViewModel _petInfo = new(new());
    private List<ComboBoxNode<int>> _skillTypeSelection = new() {
        new("全部",0),
        new("手动",1),
        new("自动",2),
        new("buff",3)
    };
    private string _searchText = string.Empty;
    private int _searchSkillType = 0;
    private PetSkillViewModel? _selectedSkill;
    private SortedDictionary<int, PetSkillViewModel> _allSkills;
    /// <summary>
    /// 数据库连接
    /// </summary>
    public DbConnection? Connection;
    #endregion
    #region Properties
    public string WindowTitle => $"修改 {_petInfo.PetName} (ID: {_petInfo.Id})技能列表";
    public PetLogViewModel PetInfo
    {
        set
        {
            _inputPetInfo = value;
            _petInfo.CopyFrom(value);
            RaisePropertyChanged(nameof(WindowTitle));
            LoadSkillList(value.Skill);
            NotifyReloadSkillSelection();
        }
    }
    public List<ComboBoxNode<int>> SkillTypeSelection => _skillTypeSelection;

    public int SearchSkillType
    {
        get => _searchSkillType;
        set
        {
            if (SetProperty(ref _searchSkillType, value))
            {
                NotifyReloadSkillSelection();
            }
        }
    }
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                NotifyReloadSkillSelection();
            }
        }
    }

    public PetSkillViewModel? SelectedSkill
    {
        get => _selectedSkill;
        set => SetProperty(ref _selectedSkill, value);
    }

    public List<PetSkillViewModel> SkillSelection
    {
        get
        {
            return (from skillItem in _allSkills.Values
                        //类别筛选
                    where _searchSkillType == 0 || skillItem.SkillType == (_searchSkillType - 1)
                    //关键词筛选
                    where skillItem.Name.IndexOf(_searchText) >= 0
                    //排除已存在的
                    where !SkillList.Contains(skillItem)
                    select skillItem).ToList();
        }
    }

    public ObservableCollection<PetSkillViewModel> SkillList { get; } = new();

    public bool IsSaving
    {
        get => _isSaving;
        set
        {
            if (SetProperty(ref _isSaving, value))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public Command SaveCommand { get; }
    public Command AddPetSkillCommand { get; }
    public Command DeletePetSkillCommand { get; }
    #endregion

    public PetSkillEditorViewModel()
    {
        SaveCommand = new(SavePetSkill, () => !_isSaving);
        AddPetSkillCommand = new(AddSkillToList, CanAddSkill);
        DeletePetSkillCommand = new(DeletePetSkill);
        _allSkills = new();
        foreach (var keyPair in SharedData.PetSkillMap)
        {
            _allSkills[keyPair.Key] = new(keyPair.Value);
        }
    }

    /// <summary>
    /// 重新加载列表
    /// </summary>
    private void NotifyReloadSkillSelection()
    {
        RaisePropertyChanged(nameof(SkillSelection));
        //默认选择第一个
        SelectedSkill = SkillSelection.First();
        AddPetSkillCommand.RaiseCanExecuteChanged();
    }

    private void LoadSkillList(string skillHex)
    {
        if (Connection is null)
        {
            return;
        }
        var serverType = Connection.GameServerType;
        var pData = DataService.ConvertToPData(skillHex);
        for (var i = 0; i < 13; i++)
        {
            var offset = i;
            byte flag;
            if (serverType == ServerType.Common)
            {
                offset *= 3;
                flag = pData[offset];
                offset++;
            }
            else
            {
                offset *= 5;
                flag = pData[offset + 4];
            }
            if (flag == 0)
            {
                continue;
            }
            int skillId;
            if (serverType == ServerType.Common)
            {
                skillId = DataService.ReadShort(pData, offset);
            }
            else
            {
                skillId = DataService.ReadInt(pData, offset);
            }
            if (_allSkills.TryGetValue(skillId, out var skillItem))
            {
                SkillList.Add(skillItem);
            }
        }
    }

    private async void SavePetSkill()
    {
        if (Connection is null)
        {
            return;
        }
        IsSaving = true;
        try
        {
            await Task.Run(async () =>
            {
                await DoSavePetSkillAsync(Connection, _petInfo);
            });
            _inputPetInfo?.CopyFrom(_petInfo);
            ShowMessage("保存成功", "保存珍兽技能成功");
            OwnedWindow?.Close();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("保存珍兽技能失败", ex);
        }
        finally
        {
            IsSaving = false;
        }
    }

    private async Task DoSavePetSkillAsync(DbConnection connection, PetLogViewModel petInfo)
    {
        int nodeLength;
        var serverType = connection.GameServerType;
        if (serverType == ServerType.Common)
        {
            nodeLength = 3;
        }
        else
        {
            nodeLength = 5;
        }

        var pData = new byte[13 * nodeLength];
        var offset = 0;
        //写入技能id
        foreach (var skillInfo in SkillList)
        {
            if (serverType == ServerType.Common)
            {
                pData[offset] = 1;
                offset++;
                DataService.WriteData(pData, offset, (short)skillInfo.Id);
                offset += 2;
            }
            else
            {
                DataService.WriteData(pData, offset, skillInfo.Id);
                offset += 4;
                pData[offset] = 1;
                offset++;
            }
        }
        //填充剩余数据
        while (offset < pData.Length)
        {

            if (serverType == ServerType.Common)
            {
                short padValue = -1;
                pData[offset] = 0;
                offset++;
                DataService.WriteData(pData, offset, padValue);
                offset += 2;
            }
            else
            {
                int padValue = -1;
                DataService.WriteData(pData, offset, padValue);
                offset += 4;
                pData[offset] = 0;
                offset++;
            }
        }
        petInfo.Skill = DataService.ConvertToHex(pData);
        //
        const string sql = "UPDATE t_pet SET skill=@skill WHERE aid=@aid";
        var mySqlCommand = new MySqlCommand(sql, connection.Conn);
        mySqlCommand.Parameters.Add(new MySqlParameter("@skill", MySqlDbType.String)
        {
            Value = petInfo.Skill
        });
        mySqlCommand.Parameters.Add(new MySqlParameter("@aid", MySqlDbType.Int32)
        {
            Value = petInfo.Id
        });
        // 切换数据库
        await connection.SwitchGameDbAsync();
        //exec
        await mySqlCommand.ExecuteNonQueryAsync();
    }

    private bool CanAddSkill()
    {
        if (_isSaving)
        {
            return false;
        }
        if (_selectedSkill == null)
        {
            return false;
        }

        const int maxSkillCount = 12;
        return SkillList.Count < maxSkillCount;
    }

    private void AddSkillToList()
    {
        if (_selectedSkill is null)
        {
            return;
        }

        //判断是否存在
        foreach (var skillInfo in SkillList)
        {
            if (skillInfo.Id == _selectedSkill.Id)
            {
                return;
            }
        }

        const int maxSkillCount = 12;
        if (SkillList.Count >= maxSkillCount)
        {
            return;
        }

        SkillList.Add(_selectedSkill);
        NotifyReloadSkillSelection();
    }

    private void DeletePetSkill(object? parameter)
    {
        if (parameter is PetSkillViewModel targetSkill)
        {
            SkillList.Remove(targetSkill);
            NotifyReloadSkillSelection();
        }
    }
}
