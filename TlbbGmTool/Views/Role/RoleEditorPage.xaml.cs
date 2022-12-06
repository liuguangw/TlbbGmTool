using liuguang.TlbbGmTool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace liuguang.TlbbGmTool.Views.Role;

public partial class RoleEditorPage : Page
{
    private bool _vmBind = false;
    public RoleEditorPage()
    {
        InitializeComponent();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        var vm = (RoleEditorViewModel)DataContext;
        if (!_vmBind)
        {
            _vmBind = true;
            var roleWindow = Window.GetWindow(this);
            var roleWindowVm = (RoleWindowViewModel)roleWindow.DataContext;
            vm.OwnedWindow = roleWindow;
            vm.Connection = roleWindowVm.Connection;
            if (roleWindowVm.RoleInfo != null)
            {
                vm.RoleInfo = roleWindowVm.RoleInfo;
            }
        }
        await vm.LoadRoleInfoAsync();
    }
}
