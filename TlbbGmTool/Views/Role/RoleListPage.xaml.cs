using liuguang.TlbbGmTool.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace liuguang.TlbbGmTool.Views.Role;

/// <summary>
/// RoleListPage.xaml 的交互逻辑
/// </summary>
public partial class RoleListPage : Page
{
    private bool _vmBind = false;
    public RoleListPage()
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
        var vm = (RoleListViewModel)DataContext;
        vm.OwnedWindow = mainWindow;
        vm.Connection = mainWindowVm.MainModel.Connection;
        vm.MenpaiMap = mainWindowVm.MainModel.MenpaiMap;
        //MessageBox.Show("bind2");
        mainWindowVm.PropertyChanged += (sender, evt) =>
        {
            //连接被断开后,清理搜索结果列表
            if (evt.PropertyName == nameof(mainWindowVm.CanDisConnServer))
            {
                if (mainWindowVm.CanConnServer)
                {
                    vm.RoleList.Clear();
                    //MessageBox.Show("clear2...");
                }
            }
        };
    }
}
