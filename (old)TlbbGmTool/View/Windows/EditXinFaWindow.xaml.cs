using System.Windows;
using TlbbGmTool.Models;
using TlbbGmTool.ViewModels;

namespace TlbbGmTool.View.Windows
{
    public partial class EditXinFaWindow : Window
    {
        public EditXinFaWindow(MainWindowViewModel mainWindowViewModel, XinFa xinFaInfo)
        {
            InitializeComponent();
            GetViewModel().InitData(mainWindowViewModel, xinFaInfo, this);
        }

        private EditXinFaViewModel GetViewModel()
        {
            return DataContext as EditXinFaViewModel;
        }
    }
}