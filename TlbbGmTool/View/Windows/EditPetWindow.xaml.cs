using System.Windows;
using TlbbGmTool.Models;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Windows
{
    public partial class EditPetWindow : Window
    {
        public EditPetWindow(MainWindowViewModel mainWindowViewModel, Pet petInfo)
        {
            InitializeComponent();
            GetViewModel().InitData(mainWindowViewModel, petInfo, this);
        }

        private EditPetViewModel GetViewModel()
        {
            return DataContext as EditPetViewModel;
        }
    }
}