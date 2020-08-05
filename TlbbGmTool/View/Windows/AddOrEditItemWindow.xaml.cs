using System.Collections.ObjectModel;
using System.Windows;
using TlbbGmTool.Models;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Windows
{
    public partial class AddOrEditItemWindow : Window
    {
        public AddOrEditItemWindow(MainWindowViewModel mainWindowViewModel,
            ItemInfo itemInfo, bool isMaterial, int charguid, ObservableCollection<ItemInfo> itemList)
        {
            InitializeComponent();
            GetViewModel().InitData(mainWindowViewModel, itemInfo, charguid, isMaterial, itemList, this);
        }

        private AddOrEditItemViewModel GetViewModel()
        {
            return DataContext as AddOrEditItemViewModel;
        }
    }
}