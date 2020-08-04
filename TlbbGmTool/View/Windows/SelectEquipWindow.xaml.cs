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

        public SelectEquipWindow(List<ItemBase> equipBaseList, int itemBaseId,
            bool sameEquipPoint = false)
        {
            InitializeComponent();
            var equipBaseInfo = (from baseInfo in equipBaseList
                where baseInfo.Id == itemBaseId
                select baseInfo).First();
            EquipBaseInfo = equipBaseInfo;
            GetViewModel().InitData(equipBaseList, this, equipBaseInfo, sameEquipPoint);
        }

        private SelectEquipViewModel GetViewModel()
        {
            return DataContext as SelectEquipViewModel;
        }
    }
}