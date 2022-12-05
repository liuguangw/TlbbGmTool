using System.Windows;
using System.Windows.Controls;
using TlbbGmTool.View.Windows;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Pages
{
    public partial class PetList : Page
    {
        public PetList()
        {
            InitializeComponent();
        }

        private PetListViewModel GetViewModel()
        {
            return DataContext as PetListViewModel;
        }

        private EditRoleWindow GetEditRoleWindow()
        {
            return Window.GetWindow(this) as EditRoleWindow;
        }

        private void PetList_OnLoaded(object sender, RoutedEventArgs e)
        {
            var editRoleWindow = GetEditRoleWindow();
            var editRoleWindowViewModel = editRoleWindow.DataContext as EditRoleWindowViewModel;
            var mainWindowViewModel = editRoleWindowViewModel.MainWindowViewModel;
            var charguid = editRoleWindowViewModel.GameRole.Charguid;
            GetViewModel().InitData(mainWindowViewModel, charguid, editRoleWindow);
        }
    }
}