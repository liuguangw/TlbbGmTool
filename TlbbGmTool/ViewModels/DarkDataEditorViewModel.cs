using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Services;
using liuguang.TlbbGmTool.ViewModels.Data;
using liuguang.TlbbGmTool.Views.Item;
using System.Linq;

namespace liuguang.TlbbGmTool.ViewModels;
public class DarkDataEditorViewModel : ViewModelBase
{
    #region Fields
    private readonly DarkDataViewModel _darkData = new();
    #endregion

    #region Properties
    public string InitHexData
    {
        set
        {
            if (value.Length == 56)
            {
                var pData = DataService.ConvertToPData(value);
                DarkDataService.Read(pData, _darkData);
            }
        }
    }
    public DarkDataViewModel DarkData => _darkData;
    #endregion
    #region Commands
    public Command SaveCommand { get; }
    public Command SelectImpact0Command { get; }
    public Command SelectImpact1Command { get; }
    public Command SelectImpact2Command { get; }
    #endregion
    public DarkDataEditorViewModel()
    {
        SaveCommand = new(ConfirmDarkData);
        SelectImpact0Command = new(() => ShowImpactSelector(0));
        SelectImpact1Command = new(() => ShowImpactSelector(1));
        SelectImpact2Command = new(() => ShowImpactSelector(2));
    }
    private void ConfirmDarkData()
    {
        var pData = new byte[28];
        DarkDataService.Write(_darkData, pData);
        if (OwnedWindow is DarkDataEditorWindow editorWindow)
        {
            editorWindow.HexData = DataService.ConvertToHex(pData);
            editorWindow.DialogResult = true;
            editorWindow.Close();
        }

    }
    private void ShowImpactSelector(int impactIndex)
    {
        short impactId;
        switch (impactIndex)
        {
            case 0:
                impactId = _darkData.Impact0;
                break;
            case 1:
                impactId = _darkData.Impact1;
                break;
            case 2:
                impactId = _darkData.Impact2;
                break;
            default:
                ShowErrorMessage("出错了", $"无效的技能位置: {impactIndex}");
                return;
        }
        var selectorWindow = new DarkImpactSelectorWindow();
        var beforeAction = (DarkImpactSelectorViewModel vm) =>
        {
            vm.InitItemId = impactId;
            vm.ItemList = (from keyPair in SharedData.DarkImpactMap
                           select new ComboBoxNode<int>(keyPair.Value, keyPair.Key)).ToList();
        };
        if (ShowDialog(selectorWindow, beforeAction) == true)
        {
            var selectedItem = selectorWindow.SelectedItem;
            if (selectedItem != null)
            {
                var selectedId = (short)selectedItem.Value;
                switch (impactIndex)
                {
                    case 0:
                        _darkData.Impact0 = selectedId;
                        break;
                    case 1:
                        _darkData.Impact1 = selectedId;
                        break;
                    case 2:
                        _darkData.Impact2 = selectedId;
                        break;
                }
            }
        }
    }
}
