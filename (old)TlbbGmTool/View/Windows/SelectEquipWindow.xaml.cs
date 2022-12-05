using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TlbbGmTool.Models;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Windows
{
    public partial class SelectEquipWindow : Window
    {
        #region Properties

        public ItemBase EquipBaseInfo { get; set; }

        #endregion

        public SelectEquipWindow(List<ItemBase> equipBaseList, int initItemId,
            bool sameEquipPoint = false)
        {
            InitializeComponent();
            GetViewModel().InitData(equipBaseList, this, initItemId, sameEquipPoint);
        }

        private SelectEquipViewModel GetViewModel()
        {
            return DataContext as SelectEquipViewModel;
        }
    }
}