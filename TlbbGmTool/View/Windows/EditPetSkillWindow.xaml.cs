using System.Windows;
using TlbbGmTool.Models;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Windows
{
    public partial class EditPetSkillWindow : Window
    {
        public EditPetSkillWindow(MainWindowViewModel mainWindowViewModel, Pet petInfo)
        {
            InitializeComponent();
            GetViewModel().InitData(petInfo, mainWindowViewModel, this);
        }

        private EditPetSkillViewModel GetViewModel()
        {
            return DataContext as EditPetSkillViewModel;
        }
    }
}