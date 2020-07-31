using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace TlbbGmTool
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
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
            Application.Current.Shutdown();
        }
    }
}