using System.Windows;
using System.Windows.Controls;
using TlbbGmTool.Models;
using TlbbGmTool.View.Windows;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Pages
{
    public partial class XinFaList : Page
    {
        public XinFaList()
        {
            InitializeComponent();
        }

        public XinFaListViewModel GetViewModel()
        {
            return DataContext as XinFaListViewModel;
        }


        private EditRoleWindow GetEditRoleWindow()
        {
            return Window.GetWindow(this) as EditRoleWindow;
        }

        private void XinFaList_OnLoaded(object sender, RoutedEventArgs e)
        {
            var editRoleWindow = GetEditRoleWindow();
            var editRoleWindowViewModel = editRoleWindow.DataContext as EditRoleWindowViewModel;
            var mainWindowViewModel = editRoleWindowViewModel.MainWindowViewModel;
            var charguid = editRoleWindowViewModel.GameRole.Charguid;
            GetViewModel().InitData(mainWindowViewModel, charguid, editRoleWindow);
        }
    }
}