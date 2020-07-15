using System.Windows;
using TlbbGmTool.Models;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Windows
{
    public partial class EditAccountWindow : Window
    {
        public EditAccountWindow(AccountListViewModel accountListViewModel, UserAccount userAccount)
        {
            InitializeComponent();
            GetViewModel().InitData(accountListViewModel, userAccount);
        }

        private EditAccountViewModel GetViewModel()
        {
            return DataContext as EditAccountViewModel;
        }
    }
}