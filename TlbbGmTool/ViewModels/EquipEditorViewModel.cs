using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Services;
using liuguang.TlbbGmTool.ViewModels.Data;
using System.Collections.Generic;
using System.ComponentModel;

namespace liuguang.TlbbGmTool.ViewModels;
public class EquipEditorViewModel : ViewModelBase
{
    #region Fields
    private bool _isSaving = false;
    private ItemLogViewModel? _inputItemLog;
    private EquipDataViewModel _equipData = new();
    /// <summary>
    /// 数据库连接
    /// </summary>
    public DbConnection? Connection;
    #endregion
    #region Properties
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
    public string WindowTitle
    {
        get
        {
            if (_inputItemLog is null)
            {
                return "发放装备";
            }
            return "修改装备 " + _equipData.EquipName;
        }
    }
    public ItemLogViewModel ItemLog
    {
        set
        {
            _inputItemLog = value;
            EquipDataService.Read(value.ItemBaseId, value.PData, _equipData);
        }
    }
    public EquipDataViewModel EquipData => _equipData;
    /// <summary>
    /// 星级选择
    /// </summary>
    public List<ComboBoxNode<byte>> StarSection { get; } = new();
    /// <summary>
    /// 孔数选择
    /// </summary>
    public List<ComboBoxNode<byte>> GemMaxCountSection { get; } = new();
    #endregion

    #region Commands
    public Command SelectEquipCommand { get; }
    public Command SelectVisualCommand { get; }
    public Command SelectGem0Command { get; }
    public Command SelectGem1Command { get; }
    public Command SelectGem2Command { get; }
    public Command SelectGem3Command { get; }
    public Command SelectAttrCommand { get; }
    public Command SaveCommand { get; }
    #endregion

    public EquipEditorViewModel()
    {
        SelectEquipCommand = new(() => { });
        SelectVisualCommand = new(() => { });
        SelectGem0Command = new(() => { }, () => _equipData.GemMaxCount > 0);
        SelectGem1Command = new(() => { }, () => _equipData.GemMaxCount > 1);
        SelectGem2Command = new(() => { }, () => _equipData.GemMaxCount > 2);
        SelectGem3Command = new(() => { }, () => _equipData.GemMaxCount > 3);
        SelectAttrCommand = new(() => { });
        SaveCommand = new(SaveItem, () => !_isSaving);
        for (byte i = 0; i <= 9; i++)
        {
            StarSection.Add(new($"{i}星", i));
        }
        for (byte i = 0; i <= 4; i++)
        {
            GemMaxCountSection.Add(new($"{i}孔", i));
        }
        _equipData.PropertyChanged += EquipData_PropertyChanged;
    }

    private void EquipData_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        //当孔数变化后,通知选择宝石按钮的状态更新
        if (e.PropertyName == nameof(_equipData.GemMaxCount))
        {
            SelectGem0Command.RaiseCanExecuteChanged();
            SelectGem1Command.RaiseCanExecuteChanged();
            SelectGem2Command.RaiseCanExecuteChanged();
            SelectGem3Command.RaiseCanExecuteChanged();
        }
        else if (e.PropertyName == nameof(_equipData.EquipName))
        {
            RaisePropertyChanged(nameof(WindowTitle));
        }
    }

    private void SaveItem()
    {
        ShowMessage("debug", $"gem2={_equipData.Gem2}");
    }
}
