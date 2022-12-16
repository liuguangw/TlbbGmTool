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
        if (DataContext is MainWindowViewModel vm)
        {
            await vm.LoadDataAsync();
        }
    }

    private async void Window_Closed(object sender, System.EventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            await vm.FreeResourceAsync();
        }
    }
}
