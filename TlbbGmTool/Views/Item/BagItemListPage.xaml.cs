using liuguang.TlbbGmTool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace liuguang.TlbbGmTool.Views.Item;

public partial class BagItemListPage : Page
{
    private bool _vmBind = false;
    public int BagType { get; set; } = 0;
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
            vm.BagType = BagType;
            if (roleWindowVm.RoleInfo != null)
            {
                vm.CharGuid = roleWindowVm.RoleInfo.CharGuid;
            }
        }
        await vm.LoadItemListAsync();
    }
}
