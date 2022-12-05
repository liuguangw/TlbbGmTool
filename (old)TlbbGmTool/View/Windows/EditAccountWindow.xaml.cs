using System.Windows;
using TlbbGmTool.Models;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Windows
{
    public partial class EditAccountWindow : Window
    {
        public EditAccountWindow(MainWindowViewModel mainWindowViewModel, UserAccount userAccount)
        {
            InitializeComponent();
            GetViewModel().InitData(mainWindowViewModel, userAccount, this);
        }

        private EditAccountViewModel GetViewModel()
        {
            return DataContext as EditAccountViewModel;
        }
    }
}