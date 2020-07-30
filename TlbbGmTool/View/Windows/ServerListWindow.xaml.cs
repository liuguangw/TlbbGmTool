using System.Windows;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Windows
{
    public partial class ServerListWindow : Window
    {
        public ServerListWindow(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            GetViewModel().InitData(mainWindowViewModel, this);
        }

        private ServerListViewModel GetViewModel()
        {
            return DataContext as ServerListViewModel;
        }
    }
}