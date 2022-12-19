using System.Reflection;

namespace liuguang.TlbbGmTool.ViewModels;

public class AboutWindowViewModel
{
    public string Version { get; set; } = "-";
    public string AppRuntime { get; set; } = "-";
    public string RepositoryUrl { get; set; } = "-";
    public AboutWindowViewModel()
    {
        var currentAssem = Assembly.GetExecutingAssembly();
        if (currentAssem != null)
        {
            var assemName = currentAssem.GetName();
            Version = assemName?.Version?.ToString() ?? "-";
            var metaAttribute = currentAssem.GetCustomAttribute<AssemblyMetadataAttribute>();
            RepositoryUrl = metaAttribute?.Value ?? "-";

        }
#if NET
        AppRuntime = ".NET";
#elif NETFRAMEWORK
        AppRuntime = ".NET Framework";
#endif
    }
}
