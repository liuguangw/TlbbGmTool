using liuguang.TlbbGmTool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace liuguang.TlbbGmTool.Views.XinFa;

public partial class XinFaListPage : Page
{
    private bool _vmBind = false;
    public XinFaListPage()
    {
        InitializeComponent();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        var vm = (XinFaListViewModel)DataContext;
        if (!_vmBind)
        {
            _vmBind = true;
            var roleWindow = Window.GetWindow(this);
            var roleWindowVm = (RoleWindowViewModel)roleWindow.DataContext;
            vm.OwnedWindow = roleWindow;
            vm.Connection = roleWindowVm.Connection;
            if (roleWindowVm.RoleInfo != null)
            {
                vm.CharGuid = roleWindowVm.RoleInfo.CharGuid;
            }
        }
        await vm.LoadXinFaListAsync();

    }
}
