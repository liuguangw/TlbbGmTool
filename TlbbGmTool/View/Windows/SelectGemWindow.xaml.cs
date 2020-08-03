using System.Collections.Generic;
using System.Windows;
using TlbbGmTool.Models;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Windows
{
    public partial class SelectGemWindow : Window
    {
        #region Properties

        public int GemId { get; set; } = 0;

        #endregion

        public SelectGemWindow(List<ItemBase> gemList, int gemId)
        {
            InitializeComponent();
            GemId = gemId;
            GetViewModel().InitData(gemList, this, gemId);
        }

        private SelectGemViewModel GetViewModel()
        {
            return DataContext as SelectGemViewModel;
        }
    }
}