using liuguang.TlbbGmTool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace liuguang.TlbbGmTool.Views.Item;

public partial class MaterialItemListPage : Page
{
    private bool _vmBind = false;
    public MaterialItemListPage()
    {
        InitializeComponent();
    }
    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        var vm = (ItemListViewModel)DataContext;
        if (!_vmBind)
        {
            _vmBind = true;
            var roleWindow = Window.GetWindow(this);
            var roleWindowVm = (RoleWindowViewModel)roleWindow.DataContext;
            vm.OwnedWindow = roleWindow;
            vm.Connection = roleWindowVm.Connection;
            vm.PosOffset = 30;
            if (roleWindowVm.RoleInfo != null)
            {
                vm.CharGuid = roleWindowVm.RoleInfo.CharGuid;
            }
        }
        await vm.LoadItemListAsync();
    }
}
