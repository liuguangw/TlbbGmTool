using liuguang.TlbbGmTool.ViewModels.Data;
using System.Windows;

namespace liuguang.TlbbGmTool.Views.Item;

public partial class ItemSelectorWindow : Window
{

    public ItemBaseViewModel? SelectedItem { get; set; }

    public ItemSelectorWindow()
    {
        InitializeComponent();
    }
}
