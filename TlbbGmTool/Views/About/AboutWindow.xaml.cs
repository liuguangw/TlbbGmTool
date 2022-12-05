using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace liuguang.TlbbGmTool.Views.About;
public partial class AboutWindow : Window
{
    public string AppRuntime
    {
        get
        {
            var appRuntime = "未知";
#if NET
    appRuntime = ".NET";
#elif NETFRAMEWORK
            appRuntime = ".NET Framework";
#endif
            return appRuntime;
        }
    }


    public AboutWindow()
    {
        InitializeComponent();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(e.Uri.AbsoluteUri);
    }
}