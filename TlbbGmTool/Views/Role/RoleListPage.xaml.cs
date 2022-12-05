using liuguang.TlbbGmTool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace liuguang.TlbbGmTool.Views.Role;

/// <summary>
/// RoleListPage.xaml 的交互逻辑
/// </summary>
public partial class RoleListPage : Page
{
    public RoleListPage()
    {
        InitializeComponent();
    }
    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        var mainWindow = Window.GetWindow(this);
        var mainWindowVm = (MainWindowViewModel)mainWindow.DataContext;
        var vm = (RoleListViewModel)DataContext;
        vm.OwnedWindow = mainWindow;
        vm.Connection = mainWindowVm.MainModel.Connection;
        vm.MenpaiMap = mainWindowVm.MainModel.MenpaiMap;
    }
}
