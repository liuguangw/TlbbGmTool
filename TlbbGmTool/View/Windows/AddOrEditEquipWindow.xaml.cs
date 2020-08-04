using System.Windows;
using TlbbGmTool.Models;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Windows
{
    public partial class AddOrEditEquipWindow : Window
    {
        public AddOrEditEquipWindow(MainWindowViewModel mainWindowViewModel,
            ItemInfo itemInfo, int charguid)
        {
            InitializeComponent();
            GetViewModel().InitData(mainWindowViewModel, itemInfo, charguid, this);
        }

        private AddOrEditEquipViewModel GetViewModel()
        {
            return DataContext as AddOrEditEquipViewModel;
        }
    }
}