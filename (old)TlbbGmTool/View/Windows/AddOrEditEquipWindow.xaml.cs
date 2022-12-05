using System.Collections.ObjectModel;
using System.Windows;
using TlbbGmTool.Models;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Windows
{
    public partial class AddOrEditEquipWindow : Window
    {
        public AddOrEditEquipWindow(MainWindowViewModel mainWindowViewModel,
            ItemInfo itemInfo, int charguid, ObservableCollection<ItemInfo> itemList)
        {
            InitializeComponent();
            GetViewModel().InitData(mainWindowViewModel, itemInfo, charguid, itemList, this);
        }

        private AddOrEditEquipViewModel GetViewModel()
        {
            return DataContext as AddOrEditEquipViewModel;
        }
    }
}