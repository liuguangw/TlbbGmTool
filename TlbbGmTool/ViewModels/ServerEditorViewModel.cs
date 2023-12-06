using liuguang.TlbbGmTool.Common;
using System.Windows.Forms;
using System;
using System.Linq;
using liuguang.TlbbGmTool.Services;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.IO;

namespace liuguang.TlbbGmTool.ViewModels;

public class ServerEditorViewModel : ViewModelBase
{
    #region Fields
    private bool _isConnecting = false;
    private GameServerViewModel? _inputServerInfo;
    private readonly GameServerViewModel _serverInfo = new(new());
    #endregion

    #region Properties
    public GameServerViewModel ServerInfo
    {
        get => _serverInfo;
        set
        {
            _inputServerInfo = value;
            _serverInfo.CopyFrom(value);
            RaisePropertyChanged(nameof(WindowTitle));
        }
    }
    public ObservableCollection<GameServerViewModel>? ServerList { get; set; }

    public string WindowTitle => (_inputServerInfo is null) ? "添加服务器" : "修改服务器";

    public Command SaveServerCommand { get; }

    public Command ConnTestCommand { get; }

    public Command ChoseFolderCommand { get; }
    #endregion

    public ServerEditorViewModel()
    {
        ConnTestCommand = new(TryToConnect, CanTestConn);
        ChoseFolderCommand = new(ShowFolderDialog);
        SaveServerCommand = new(SaveServerConfig, CanSave);
        ServerInfo.PropertyChanged += (sender, e) =>
        {
            SaveServerCommand.RaiseCanExecuteChanged();
            ConnTestCommand.RaiseCanExecuteChanged();
        };
    }

    private void ShowFolderDialog()
    {
        using var dialog = new FolderBrowserDialog();
        if (!string.IsNullOrEmpty(ServerInfo.ClientPath))
        {
            if (Directory.Exists(ServerInfo.ClientPath))
            {
                dialog.SelectedPath = ServerInfo.ClientPath;
            }
        }
        var result = dialog.ShowDialog();
        if (result == DialogResult.OK)
        {
            ServerInfo.ClientPath = dialog.SelectedPath;
        }
    }
    private bool CanSave()
    {
        return !(string.IsNullOrEmpty(ServerInfo.ServerName) || string.IsNullOrEmpty(ServerInfo.DbHost)
            || string.IsNullOrEmpty(ServerInfo.AccountDbName) || string.IsNullOrEmpty(ServerInfo.GameDbName)
            || string.IsNullOrEmpty(ServerInfo.DbUser) || string.IsNullOrEmpty(ServerInfo.ClientPath));
    }


    private bool CanTestConn()
    {
        return !(_isConnecting || string.IsNullOrEmpty(ServerInfo.DbHost)
            || string.IsNullOrEmpty(ServerInfo.AccountDbName) || string.IsNullOrEmpty(ServerInfo.GameDbName)
            || string.IsNullOrEmpty(ServerInfo.DbUser));
    }

    private async void SaveServerConfig()
    {
        //检测客户端目录是否有效
        var configAxpPath = Path.Combine(ServerInfo.ClientPath, "Data", "Config.axp");
        if (!File.Exists(configAxpPath))
        {
            ShowErrorMessage("无效的路径", $"客户端路径[{ServerInfo.ClientPath}]无效");
            return;
        }
        //
        if (_inputServerInfo is null)
        {
            //add
            ServerList?.Add(ServerInfo);
        }
        else
        {
            //update
            _inputServerInfo.CopyFrom(ServerInfo);
        }
        //保存配置
        var serverList = from item in ServerList select item.AsServer();
        if (serverList is null)
        {
            return;
        }
        try
        {
            await ServerService.SaveGameServersAsync(serverList);
        }
        catch (Exception ex)
        {
            ShowErrorMessage("保存配置失败", ex);
            return;
        }
        OwnedWindow?.Close();
    }

    private async void TryToConnect()
    {
        _isConnecting = true;
        ConnTestCommand.RaiseCanExecuteChanged();
        var connectionStringBuilder = new MySqlConnectionStringBuilder
        {
            Server = ServerInfo.DbHost,
            Port = ServerInfo.DbPort,
            Database = ServerInfo.AccountDbName,
            UserID = ServerInfo.DbUser,
            Password = ServerInfo.DbPassword,
            ConnectionTimeout = 20,
        };
        if (ServerInfo.DisabledSsl)
        {
            connectionStringBuilder.SslMode = MySqlSslMode.Disabled;
        }

        var mySqlConnection = new MySqlConnection
        {
            ConnectionString = connectionStringBuilder.GetConnectionString(true),
        };
        try
        {
            //在后台运行，防止阻塞ui线程
            await Task.Run(async () =>
            {
                await mySqlConnection.OpenAsync();
                await mySqlConnection.CloseAsync();

            });
        }
        catch (Exception e)
        {
            ShowErrorMessage("连接失败", e);
            return;
        }
        finally
        {

            _isConnecting = false;
            ConnTestCommand.RaiseCanExecuteChanged();
        }
        ShowMessage("连接成功", "连接数据库成功");
    }
}
