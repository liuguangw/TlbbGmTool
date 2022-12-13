using liuguang.TlbbGmTool.Common;
using System.Windows.Media;

namespace liuguang.TlbbGmTool.ViewModels.Data;
public class EquipAttributeNode : NotifyBase
{
    #region Fields

    private bool _enabled = true;
    private bool _checked = false;

    #endregion

    #region Properties

    public string Name { get; }
    public int Index { get; }

    public bool Checked
    {
        get => _checked;
        set
        {
            if (SetProperty(ref _checked, value))
            {
                RaisePropertyChanged(nameof(Color));
            }
        }
    }

    public SolidColorBrush Color => _checked ? Brushes.LightSkyBlue : Brushes.White;

    public bool Enabled
    {
        get => _enabled;
        set => SetProperty(ref _enabled, value);
    }

    #endregion

    public EquipAttributeNode(string name, int index)
    {
        Name = name;
        Index = index;
    }
}
