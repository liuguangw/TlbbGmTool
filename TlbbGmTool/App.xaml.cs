global using MySqlConnector;
using System.Windows;
using System.Windows.Threading;

namespace liuguang.TlbbGmTool;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var messageContent = string.Empty;
        var ex = e.Exception;
        while (ex != null)
        {
            if (!string.IsNullOrEmpty(messageContent))
            {
                messageContent += "\n\n";
            }

            messageContent += $"{ex.Message}\n{ex.StackTrace}";
            ex = ex.InnerException;
        }

        MessageBox.Show(messageContent,
            "程序出现未捕获的异常", MessageBoxButton.OK, MessageBoxImage.Error);
        Current.Shutdown();
    }
}
