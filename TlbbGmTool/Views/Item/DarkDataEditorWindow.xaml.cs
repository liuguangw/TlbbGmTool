using System.Windows;

namespace liuguang.TlbbGmTool.Views.Item;
/// <summary>
/// DarkDataEditorWindow.xaml 的交互逻辑
/// </summary>
public partial class DarkDataEditorWindow : Window
{
    public string HexData { get; set; } = string.Empty;
    public DarkDataEditorWindow()
    {
        InitializeComponent();
    }
}
