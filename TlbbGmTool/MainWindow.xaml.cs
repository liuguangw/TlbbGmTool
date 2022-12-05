using System.Windows;
using liuguang.TlbbGmTool.ViewModels;

namespace liuguang.TlbbGmTool;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        var vm = DataContext as MainWindowViewModel;
        if (vm != null)
        {
            await vm.LoadDataAsync();
        }
    }

    private async void Window_Closed(object sender, System.EventArgs e)
    {
        var vm = DataContext as MainWindowViewModel;
        if (vm != null)
        {
            await vm.FreeResourceAsync();
        }
    }
}
