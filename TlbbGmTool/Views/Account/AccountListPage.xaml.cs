using liuguang.TlbbGmTool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace liuguang.TlbbGmTool.Views.Account;

/// <summary>
/// AccountListPage.xaml 的交互逻辑
/// </summary>
public partial class AccountListPage : Page
{
    public AccountListPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        var mainWindow = Window.GetWindow(this);
        var mainWindowVm = (MainWindowViewModel)mainWindow.DataContext;
        var vm = (AccountListViewModel)DataContext;
        vm.OwnedWindow = mainWindow;
        vm.Connection = mainWindowVm.MainModel.Connection;
    }
}
