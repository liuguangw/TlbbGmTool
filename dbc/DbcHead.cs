namespace liuguang.Dbc;

public struct DbcHead
{
    /// <summary>
    /// 标示0xDDBBCC00
    /// </summary>
    public uint Identity;
    /// <summary>
    /// 字段个数(列)
    /// </summary>
    public int FieldCount;
    /// <summary>
    /// 数据行数
    /// </summary>
    public int RecordCount;
    /// <summary>
    /// 字符串区大小
    /// </summary>
    public int StringBlockSize;
}
