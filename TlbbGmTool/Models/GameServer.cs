namespace liuguang.TlbbGmTool.Models;

public class GameServer
{
    /// <summary>
    /// 服务器名称
    /// </summary>
    public string ServerName = string.Empty;

    /// <summary>
    /// 主机名
    /// </summary>
    public string DbHost = string.Empty;

    /// <summary>
    /// 端口
    /// </summary>
    public ushort DbPort = 3306;

    /// <summary>
    /// 账号数据库名称
    /// </summary>
    public string AccountDbName = "web";

    /// <summary>
    /// 游戏数据库名称
    /// </summary>
    public string GameDbName = "tlbbdb";

    /// <summary>
    /// 数据库用户名
    /// </summary>
    public string DbUser = "root";

    /// <summary>
    /// 数据库密码
    /// </summary>
    public string DbPassword = string.Empty;

    /// <summary>
    /// 客户端路径
    /// </summary>
    public string ClientPath = string.Empty;
}