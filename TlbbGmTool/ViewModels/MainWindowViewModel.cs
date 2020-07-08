using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.Services;

namespace TlbbGmTool.ViewModels
{
    public class MainWindowViewModel : BindDataBase
    {
        public ObservableCollection<GameServer> ServerList { get; } = new ObservableCollection<GameServer>();
       
        public async Task LoadApplicationData()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var configFilePath = Path.Combine(baseDir, "config", "servers.xml");
            var servers = await ServerService.LoadGameServers(configFilePath);
            foreach (var server in servers)
            {
                ServerList.Add(server);
            }
        }
    }
}