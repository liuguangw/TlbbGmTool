using System.Collections.Generic;
using System.Windows;
using TlbbGmTool.Models;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Windows
{
    public partial class SelectItemWindow : Window
    {
        #region Properties

        public ItemBase TargetItem { get; set; }

        #endregion

        public SelectItemWindow(List<ItemBase> itemBaseList, int initItemId = 0)
        {
            InitializeComponent();
            GetViewModel().InitData(itemBaseList, this, initItemId);
        }

        private SelectItemViewModel GetViewModel()
        {
            return DataContext as SelectItemViewModel;
        }
    }
}