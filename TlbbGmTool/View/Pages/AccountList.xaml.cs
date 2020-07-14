using System.Windows;
using System.Windows.Controls;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Pages
{
    /// <summary>
    /// AccountList.xaml 的交互逻辑
    /// </summary>
    public partial class AccountList : Page
    {
        public AccountList()
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
        private AccountListViewModel GetViewModel()
        {
            return DataContext as AccountListViewModel;
        }

        private void AccountList_OnLoaded(object sender, RoutedEventArgs e)
        {
            // 存储主窗口ViewModel对象
            GetViewModel().MainWindowViewModel = GetMainWindowViewModel();
        }
    }
}