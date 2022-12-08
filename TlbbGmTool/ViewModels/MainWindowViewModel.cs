using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Models;
using liuguang.TlbbGmTool.Services;
using liuguang.TlbbGmTool.Views.About;
using liuguang.TlbbGmTool.Views.Server;

namespace liuguang.TlbbGmTool.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    #region Fields
    private MainWindowModel _mainWindowModel = new();
    public GameServerViewModel? _selectedServer;
    private DbStatus _currentDbStatus = DbStatus.NotConnect;
    #endregion

    #region Properties
    public override Window? OwnedWindow => Application.Current.MainWindow;
    /// <summary>
    /// 供Page调用
    /// </summary>
    public MainWindowModel MainModel => _mainWindowModel;
    public string WindowTitle
    {
        get
        {
            var title = "天龙八部GM工具 - by 流光";
#if DEBUG
            title = "[debug]" + title;
#endif
            if (!string.IsNullOrEmpty(_mainWindowModel.DbVersion))
            {
                title += $"(MySQL: {_mainWindowModel.DbVersion})";
            }

            if (_mainWindowModel.DataStatus == DataStatus.Loading)
            {
                title += "(加载配置中...)";
            }

            return title;
        }
    }

    private DataStatus DataStatus
    {
        set
        {
            if (SetProperty(ref _mainWindowModel.DataStatus, value))
            {
                RaisePropertyChanged(nameof(WindowTitle));
                RaisePropertyChanged(nameof(GameDataLoaded));
            }
        }
    }

    /// <summary>
    /// 游戏数据是否已经完成加载
    /// </summary>
    public bool GameDataLoaded
        => _mainWindowModel.DataStatus == DataStatus.Loaded;

    public GameServerViewModel? SelectedServer
    {
        get => _selectedServer;
        set
        {
            if (SetProperty(ref _selectedServer, value))
            {
                RaisePropertyChanged(nameof(CanConnServer));
                ConnectCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public ObservableCollection<GameServerViewModel> ServerList => _mainWindowModel.ServerList;

    private DbStatus CurrentDbStatus
    {
        set
        {
            if (_selectedServer is null)
            {
                return;
            }
            if (SetProperty(ref _currentDbStatus, value))
            {
                _selectedServer.DbStatus = value;
                RaisePropertyChanged(nameof(CanConnServer));
                RaisePropertyChanged(nameof(CanDisConnServer));
                ConnectCommand.RaiseCanExecuteChanged();
                DisConnectCommand.RaiseCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// 是否可以连接
    /// </summary>
    /// <returns></returns>
    public bool CanConnServer
    {
        get
        {
            if (_selectedServer is null)
            {
                return false;
            }
            return _selectedServer.DbStatus == DbStatus.NotConnect;
        }
    }


    /// <summary>
    /// 是否可以断开
    /// </summary>
    public bool CanDisConnServer
    {
        get
        {
            if (_selectedServer is null)
            {
                return false;
            }
            return _selectedServer.DbStatus == DbStatus.Connected;
        }
    }

    public Command ConnectCommand { get; }

    public Command DisConnectCommand { get; }

    public Command ExitCommand { get; }

    public Command ServerListCommand { get; }

    public Command AboutCommand { get; }
    #endregion


    public MainWindowViewModel()
    {
        ConnectCommand = new(ConnectDbAsync, () => CanConnServer);
        DisConnectCommand = new(DisConnectDb, () => CanDisConnServer);
        ExitCommand = new(Application.Current.Shutdown);
        ServerListCommand = new(ShowServerListWindow);
        AboutCommand = new(ShowAboutWindow);
        ServerList.CollectionChanged += (sender, e) =>
        {
            if (ServerList.Count == 0)
            {
                return;
            }
            //默认选择第一个
            if (_selectedServer is null || !ServerList.Contains(_selectedServer))
            {
                SelectedServer = ServerList.First();
            }
        };
    }

    public async void ConnectDbAsync()
    {
        if (_selectedServer is null)
        {
            return;
        }
        CurrentDbStatus = DbStatus.Connecting;
        try
        {
            await Task.Run(async () =>
            {
                await _mainWindowModel.Connection.OpenAsync(_selectedServer.AsServer());
                _mainWindowModel.DbVersion = await _mainWindowModel.Connection.CheckVersionAsync();
            });
            RaisePropertyChanged(nameof(WindowTitle));
        }
        catch (Exception e)
        {
            CurrentDbStatus = DbStatus.NotConnect;
            ShowErrorMessage("连接数据库失败", e);
            return;
        }
        CurrentDbStatus = DbStatus.Connected;
        //数据库连接成功过
        //从客户端的axp文件中加载数据
        this.DataStatus = DataStatus.Loading;
        _mainWindowModel.ItemBaseMap.Clear();
        try
        {
            await Task.Run(async () =>
            {
                var axpPath = Path.Combine(_selectedServer.ClientPath, "Data", "Config.axp");
                await AxpService.LoadDataAsync(axpPath, _mainWindowModel.ItemBaseMap, XinFaLogViewModel.XinFaMap, PetSkillEditorViewModel.PetSkillMap);
            });
            this.DataStatus = DataStatus.Loaded;
        }
        catch (Exception e)
        {
            this.DataStatus = DataStatus.NotLoad;
            ShowErrorMessage("加载axp文件失败", e, true);
        }
    }

    public async void DisConnectDb()
    {
        if (_selectedServer is null)
        {
            return;
        }
        try
        {
            await _mainWindowModel.Connection.CloseAsync();
        }
        catch (Exception e)
        {
            ShowErrorMessage("断开数据库失败", e);
        }
        _mainWindowModel.DbVersion = string.Empty;
        RaisePropertyChanged(nameof(WindowTitle));
        CurrentDbStatus = DbStatus.NotConnect;
        this.DataStatus = DataStatus.NotLoad;
    }

    /// <summary>
    /// 关闭之前,释放资源
    /// </summary>
    /// <returns></returns>
    public async Task FreeResourceAsync()
    {
        if (_selectedServer != null)
        {
            if (_selectedServer.DbStatus == DbStatus.Connected)
            {
                try
                {
                    await _mainWindowModel.Connection.CloseAsync();
                }
                catch (Exception)
                {
                }
            }
        }
    }

    /// <summary>
    /// 加载数据
    /// </summary>
    /// <returns></returns>
    public async Task LoadDataAsync()
    {
        var taskList = new Task[]{
            LoadServerListAsync(),
            CommonConfigService.LoadConfigAsync(RoleViewModel.MenpaiMap, _mainWindowModel.Attr1Map, _mainWindowModel.Attr2Map)
        };
        try
        {
            await Task.WhenAll(taskList);
        }
        catch (Exception e)
        {
            ShowErrorMessage("加载配置出错", e);
        }
    }

    /// <summary>
    /// 加载区服配置列表
    /// </summary>
    /// <returns></returns>
    private async Task LoadServerListAsync()
    {
        var servers = await ServerService.LoadServersAsync();
        foreach (var item in servers)
        {
            var server = new GameServerViewModel(item);
            ServerList.Add(server);
        }
    }

    private void ShowServerListWindow()
    {
        ShowDialog(new ServerListWindow(), (ServerListViewModel vm) =>
        {
            vm.ServerList = ServerList;
        });
    }

    private void ShowAboutWindow()
    {
        ShowDialog(new AboutWindow());
    }
}
