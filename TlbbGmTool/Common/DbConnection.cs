using System.Threading.Tasks;
using liuguang.TlbbGmTool.Models;

namespace liuguang.TlbbGmTool.Common;

public class DbConnection
{
    /// <summary>
    /// MySQL连接对象
    /// </summary>
    private MySqlConnection _conn;
    /// <summary>
    /// 账号数据库名称
    /// </summary>
    private string _accountDbName = "web";

    /// <summary>
    /// 游戏数据库名称
    /// /// </summary>
    private string _gameDbName = "tlbbdb";

    /// <summary>
    /// 当前选择的数据库
    /// </summary>
    private string? _currentDbName;

    /// <summary>
    /// 服务端类型
    /// </summary>
    public ServerType _serverType;

    /// <summary>
    /// MySQL连接对象
    /// </summary>
    public MySqlConnection Conn => _conn;

    /// <summary>
    /// 服务端类型
    /// </summary>
    public ServerType GameServerType =>_serverType;

    public DbConnection()
    {
        _serverType = ServerType.Common;
        _conn = new();
    }

    public async Task OpenAsync(GameServer serverInfo)
    {
        _serverType = serverInfo.GameServerType;
        var connectionStringBuilder = new MySqlConnectionStringBuilder
        {
            Server = serverInfo.DbHost,
            Port = serverInfo.DbPort,
            UserID = serverInfo.DbUser,
            Password = serverInfo.DbPassword,
            ConnectionTimeout = 20,
            MinimumPoolSize = 3,
            ConnectionLifeTime = 4 * 60,
            Keepalive = 30,
        };
        if (serverInfo.DisabledSsl)
        {
            connectionStringBuilder.SslMode = MySqlSslMode.Disabled;
        }
        _conn.ConnectionString = connectionStringBuilder.ConnectionString;
        await _conn.OpenAsync();
        _accountDbName = serverInfo.AccountDbName;
        _gameDbName = serverInfo.GameDbName;
        _currentDbName = null;
    }

    private async Task ChangeDataBaseAsync(string dbName)
    {
        if (_currentDbName == dbName)
        {
            return;
        }
        await _conn.ChangeDatabaseAsync(dbName);
        _currentDbName = dbName;
    }

    /// <summary>
    /// 切换到游戏数据库
    /// </summary>
    /// <returns></returns>
    public async Task SwitchGameDbAsync()
    {
        await ChangeDataBaseAsync(_gameDbName);
    }

    /// <summary>
    /// 切换到账号数据库
    /// </summary>
    /// <returns></returns>
    public async Task SwitchAccountDbAsync()
    {
        await ChangeDataBaseAsync(_accountDbName);
    }

    /// <summary>
    /// 关闭数据库连接
    /// </summary>
    /// <returns></returns>
    public async Task CloseAsync()
    {
        await _conn.CloseAsync();
    }

    /// <summary>
    /// 获取版本信息
    /// </summary>
    /// <returns></returns>
    public async Task<string> CheckVersionAsync()
    {
        const string sql = "SELECT version()";
        var mySqlCommand = new MySqlCommand(sql, _conn);
        var rd = await mySqlCommand.ExecuteScalarAsync();
        var version = rd?.ToString() ?? "";
        return version;
    }
}
