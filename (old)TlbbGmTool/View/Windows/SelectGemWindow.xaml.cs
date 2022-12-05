using System.Collections.Generic;
using System.Windows;
using TlbbGmTool.Models;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Windows
{
    public partial class SelectGemWindow : Window
    {
        #region Properties

        public int TargetItemId { get; set; } = 0;

        #endregion

        public SelectGemWindow(List<ItemBase> gemList, int initItemId = 0)
        {
            InitializeComponent();
            GetViewModel().InitData(gemList, this, initItemId);
        }

        private SelectGemViewModel GetViewModel()
        {
            return DataContext as SelectGemViewModel;
        }
    }
}