using liuguang.TlbbGmTool.ViewModels.Data;
using System.Windows;

namespace liuguang.TlbbGmTool.Views.Item;

public partial class LvItemSelectorWindow : Window
{

    public ItemBaseViewModel? SelectedItem { get; set; }

    public LvItemSelectorWindow()
    {
        InitializeComponent();
    }
}
