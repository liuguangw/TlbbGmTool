using TlbbGmTool.Core;

namespace TlbbGmTool.Models
{
    public class GameServer : BindDataBase
    {
        #region Fields

        private string _serverName = string.Empty;
        private string _dbHost = string.Empty;
        private uint _dbPort;
        private string _accountDbName = string.Empty;
        private string _gameDbName = string.Empty;
        private string _dbUser = string.Empty;
        private string _dbPassword = string.Empty;
        private bool _connected;

        #endregion

        #region Properties

        /// <summary>
        /// 服务器名称
        /// </summary>
        public string ServerName
        {
            get => _serverName;
            set => SetProperty(ref _serverName, value);
        }

        /// <summary>
        /// 主机名
        /// </summary>
        public string DbHost
        {
            get => _dbHost;
            set => SetProperty(ref _dbHost, value);
        }

        /// <summary>
        /// 端口
        /// </summary>
        public uint DbPort
        {
            get => _dbPort;
            set => SetProperty(ref _dbPort, value);
        }

        /// <summary>
        /// 账号数据库名称
        /// </summary>
        public string AccountDbName
        {
            get => _accountDbName;
            set => SetProperty(ref _accountDbName, value);
        }

        /// <summary>
        /// 游戏数据库名称
        /// </summary>
        public string GameDbName
        {
            get => _gameDbName;
            set => SetProperty(ref _gameDbName, value);
        }

        /// <summary>
        /// 数据库用户名
        /// </summary>
        public string DbUser
        {
            get => _dbUser;
            set => SetProperty(ref _dbUser, value);
        }

        /// <summary>
        /// 数据库密码
        /// </summary>
        public string DbPassword
        {
            get => _dbPassword;
            set => SetProperty(ref _dbPassword, value);
        }

        public bool Connected
        {
            get => _connected;
            set => SetProperty(ref _connected, value);
        }

        #endregion
    }
}