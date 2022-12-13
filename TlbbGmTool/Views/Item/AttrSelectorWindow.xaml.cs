using System.Windows;

namespace liuguang.TlbbGmTool.Views.Item;

/// <summary>
/// 属性选择窗体
/// </summary>
public partial class AttrSelectorWindow : Window
{
    #region Properties

    public int Attr0 { get; set; }
    public int Attr1 { get; set; }

    #endregion

    public AttrSelectorWindow()
    {
        InitializeComponent();
    }
}
