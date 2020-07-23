using System.Text;

namespace TlbbGmTool.Services
{
    /// <summary>
    /// 编码转换
    /// </summary>
    public class DbStringService
    {
        /// <summary>
        /// 文本编码: 简体中文(GB18030)
        /// </summary>
        private static readonly Encoding StrEncoding = Encoding.GetEncoding("GB18030");

        /// <summary>
        /// 数据库存储编码: 西欧语(ISO)
        /// </summary>
        private static readonly Encoding StorageEncoding = Encoding.GetEncoding("iso-8859-1");

        /// <summary>
        /// 将数据库内的字符串解码为普通字符串
        /// </summary>
        /// <param name="dbString"></param>
        /// <returns></returns>
        public static string ToCommonString(string dbString)
        {
            var bytes = StorageEncoding.GetBytes(dbString);
            return StrEncoding.GetString(bytes);
        }

        /// <summary>
        /// 将普通字符串编码为数据库中的编码
        /// </summary>
        /// <param name="commonString"></param>
        /// <returns></returns>
        public static string ToDbString(string commonString)
        {
            var bytes = StrEncoding.GetBytes(commonString);
            return StorageEncoding.GetString(bytes);
        }
    }
}