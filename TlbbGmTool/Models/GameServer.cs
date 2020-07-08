namespace TlbbGmTool.Models
{
    public class GameServer
    {
        /// <summary>
        /// 服务器名称
        /// </summary>
        public string ServerName { get; set; } = string.Empty;

        /// <summary>
        /// 主机名
        /// </summary>
        public string DbHost { get; set; } = string.Empty;

        /// <summary>
        /// 端口
        /// </summary>
        public string DbPort { get; set; } = string.Empty;

        /// <summary>
        /// 账号数据库名称
        /// </summary>
        public string AccountDbName { get; set; } = string.Empty;

        /// <summary>
        /// 游戏数据库名称
        /// </summary>
        public string GameDbName { get; set; } = string.Empty;

        /// <summary>
        /// 数据库用户名
        /// </summary>
        public string DbUser { get; set; } = string.Empty;

        /// <summary>
        /// 数据库密码
        /// </summary>
        public string DbPassword { get; set; } = string.Empty;
    }
}