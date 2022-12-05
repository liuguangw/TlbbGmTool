namespace TlbbGmTool.Models
{
    public enum DatabaseConnectionStatus
    {
        /// <summary>
        /// 未连接
        /// </summary>
        NoConnection,

        /// <summary>
        /// 正在连接 or 正在断开
        /// </summary>
        Pending,

        /// <summary>
        /// 已连接
        /// </summary>
        Connected
    }
}