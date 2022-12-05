using System.Windows;
using System.Windows.Controls;
using TlbbGmTool.View.Windows;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Pages
{
    public partial class EditRolePage : Page
    {
        public EditRolePage()
        {
            InitializeComponent();
        }

        private EditRoleViewModel GetViewModel()
        {
            return DataContext as EditRoleViewModel;
        }

        private EditRoleWindow GetEditRoleWindow()
        {
            return Window.GetWindow(this) as EditRoleWindow;
        }

        private void EditRolePage_OnLoaded(object sender, RoutedEventArgs e)
        {
            var editRoleWindow = GetEditRoleWindow();
            var editRoleWindowViewModel = editRoleWindow.DataContext as EditRoleWindowViewModel;
            GetViewModel().InitData(editRoleWindowViewModel);
        }
    }
}