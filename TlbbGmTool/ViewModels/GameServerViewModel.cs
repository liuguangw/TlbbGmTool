using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Models;

namespace liuguang.TlbbGmTool.ViewModels;

public class GameServerViewModel : NotifyBase
{
    #region Fields
    private GameServer _gameServer;
    private DbStatus _dbStatus = DbStatus.NotConnect;
    #endregion

    #region Properties
    public string ServerName
    {
        get => _gameServer.ServerName;
        set => SetProperty(ref _gameServer.ServerName, value);
    }

    public string DbHost
    {
        get => _gameServer.DbHost;
        set => SetProperty(ref _gameServer.DbHost, value);
    }

    public ushort DbPort
    {
        get => _gameServer.DbPort;
        set => SetProperty(ref _gameServer.DbPort, value);
    }

    public string AccountDbName
    {
        get => _gameServer.AccountDbName;
        set => SetProperty(ref _gameServer.AccountDbName, value);
    }

    public string GameDbName
    {
        get => _gameServer.GameDbName;
        set => SetProperty(ref _gameServer.GameDbName, value);
    }

    public string DbUser
    {
        get => _gameServer.DbUser;
        set => SetProperty(ref _gameServer.DbUser, value);
    }

    public string DbPassword
    {
        get => _gameServer.DbPassword;
        set => SetProperty(ref _gameServer.DbPassword, value);
    }

    public bool DisabledSsl
    {
        get => _gameServer.DisabledSsl;
        set => SetProperty(ref _gameServer.DisabledSsl, value);
    }

    public ServerType GameServerType
    {
        get => _gameServer.GameServerType;
        set => _gameServer.GameServerType = value;
    }

    public string ClientPath
    {
        get => _gameServer.ClientPath;
        set => SetProperty(ref _gameServer.ClientPath, value);
    }

    public DbStatus DbStatus
    {
        get => _dbStatus;
        set => SetProperty(ref _dbStatus, value);
    }
    #endregion

    public GameServerViewModel(GameServer gameServer)
    {
        _gameServer = gameServer;
    }

    public void CopyFrom(GameServerViewModel src)
    {
        ServerName = src.ServerName;
        DbHost = src.DbHost;
        DbPort = src.DbPort;
        AccountDbName = src.AccountDbName;
        GameDbName = src.GameDbName;
        DbUser = src.DbUser;
        DbPassword = src.DbPassword;
        DisabledSsl = src.DisabledSsl;
        GameServerType = src.GameServerType;
        ClientPath = src.ClientPath;
    }

    public GameServer AsServer()
    {
        return _gameServer;
    }
}
