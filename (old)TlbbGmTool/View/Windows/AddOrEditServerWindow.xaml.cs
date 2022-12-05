using System.Windows;
using TlbbGmTool.Models;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Windows
{
    public partial class AddOrEditServerWindow : Window
    {
        public AddOrEditServerWindow(GameServer serverInfo, MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            GetViewModel().InitData(mainWindowViewModel, serverInfo, this);
        }

        private AddOrEditServerViewModel GetViewModel()
        {
            return DataContext as AddOrEditServerViewModel;
        }
    }
}