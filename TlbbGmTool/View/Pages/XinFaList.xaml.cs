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
            GetViewModel().InitData(editRoleWindowViewModel);
        }

        private void ShowEditXinFaDialog(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var xinFaInfo = btn.DataContext as XinFa;
            var editRoleWindow = GetEditRoleWindow();
            var editRoleWindowViewModel = editRoleWindow.DataContext as EditRoleWindowViewModel;
            var mainWindowViewModel = editRoleWindowViewModel.MainWindowViewModel;
            var editXinFaWindow = new EditXinFaWindow(mainWindowViewModel, xinFaInfo)
            {
                Owner = GetEditRoleWindow()
            };
            editXinFaWindow.ShowDialog();
        }
    }
}