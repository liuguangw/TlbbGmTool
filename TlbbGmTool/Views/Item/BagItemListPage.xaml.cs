using liuguang.TlbbGmTool.Models;
using liuguang.TlbbGmTool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace liuguang.TlbbGmTool.Views.Item;

public partial class BagItemListPage : Page
{
    private bool _vmBind = false;
    public BagType RoleBagType { get; set; } = BagType.ItemBag;
    public BagItemListPage()
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
            vm.ItemsContainer.RoleBagType = RoleBagType;
            if (roleWindowVm.RoleInfo != null)
            {
                vm.ItemsContainer.CharGuid = roleWindowVm.RoleInfo.CharGuid;
            }
        }
        await vm.LoadItemListAsync();
    }
}
