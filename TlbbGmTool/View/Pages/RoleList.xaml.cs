using System.Windows;
using System.Windows.Controls;
using TlbbGmTool.Models;
using TlbbGmTool.View.Windows;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Pages
{
    /// <summary>
    /// RoleList.xaml 的交互逻辑
    /// </summary>
    public partial class RoleList : Page
    {
        public RoleList()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 主窗体对象
        /// </summary>
        /// <returns></returns>
        private MainWindow GetMainWindow()
        {
            return Window.GetWindow(this) as MainWindow;
        }

        /// <summary>
        /// 主窗体绑定的ViewModel对象
        /// </summary>
        /// <returns></returns>
        private MainWindowViewModel GetMainWindowViewModel()
        {
            return GetMainWindow().DataContext as MainWindowViewModel;
        }

        /// <summary>
        /// 当前Page绑定的ViewModel对象
        /// </summary>
        /// <returns></returns>
        private RoleListViewModel GetViewModel()
        {
            return DataContext as RoleListViewModel;
        }

        private void RoleList_OnLoaded(object sender, RoutedEventArgs e)
        {
            // 存储主窗口ViewModel对象
            GetViewModel().MainWindowViewModel = GetMainWindowViewModel();
        }

        private void ShowEditRoleDialog(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var gameRole = btn.DataContext as GameRole;
            var editRoleWindow = new EditRoleWindow(GetViewModel().MainWindowViewModel, gameRole)
            {
                Owner = GetMainWindow()
            };
            editRoleWindow.ShowDialog();
        }
    }
}