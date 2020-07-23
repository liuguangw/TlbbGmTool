using System.Windows;
using TlbbGmTool.Models;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Windows
{
    public partial class EditRoleWindow : Window
    {
        public EditRoleWindow(MainWindowViewModel mainWindowViewModel,GameRole gameRole)
        {
            InitializeComponent();
            GetViewModel().InitData(mainWindowViewModel, gameRole, this);
        }

        private EditRoleWindowViewModel GetViewModel()
        {
            return DataContext as EditRoleWindowViewModel;
        }
    }
}