using liuguang.TlbbGmTool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace liuguang.TlbbGmTool.Views.Account;

/// <summary>
/// AccountListPage.xaml 的交互逻辑
/// </summary>
public partial class AccountListPage : Page
{
    private bool _vmBind = false;
    public AccountListPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (_vmBind)
        {
            return;
        }
        _vmBind = true;
        //
        var mainWindow = Window.GetWindow(this);
        var mainWindowVm = (MainWindowViewModel)mainWindow.DataContext;
        var vm = (AccountListViewModel)DataContext;
        vm.OwnedWindow = mainWindow;
        vm.Connection = mainWindowVm.MainModel.Connection;
        //MessageBox.Show("bind1");
        mainWindowVm.PropertyChanged += (sender, evt) =>
        {
            //连接被断开后,清理搜索结果列表
            if (evt.PropertyName == nameof(mainWindowVm.CanDisConnServer))
            {
                if (mainWindowVm.CanConnServer)
                {
                    vm.AccountList.Clear();
                    //MessageBox.Show("clear1...");
                }
            }
        };
    }
}
