using liuguang.TlbbGmTool.Common;
using System.Windows;

namespace liuguang.TlbbGmTool.Views.Item;

public partial class DarkImpactSelectorWindow : Window
{

    public ComboBoxNode<int>? SelectedItem { get; set; }

    public DarkImpactSelectorWindow()
    {
        InitializeComponent();
    }
}
