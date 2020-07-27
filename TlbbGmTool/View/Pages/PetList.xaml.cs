using System.Windows;
using System.Windows.Controls;
using TlbbGmTool.Models;
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
            GetViewModel().InitData(editRoleWindowViewModel);
        }

        private void ShowEditPetDialog(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var petInfo = btn.DataContext as Pet;
            var editRoleWindow = GetEditRoleWindow();
            var editRoleWindowViewModel = editRoleWindow.DataContext as EditRoleWindowViewModel;
            var mainWindowViewModel = editRoleWindowViewModel.MainWindowViewModel;
            var editPetWindow = new EditPetWindow(mainWindowViewModel, petInfo)
            {
                Owner = GetEditRoleWindow()
            };
            editPetWindow.ShowDialog();
        }
    }
}