namespace liuguang.Axp;

public struct AxpHead
{
    /// <summary>
    /// 标示0x4B505841  'AXPK'
    /// </summary>
    public uint Identity;
    /// <summary>
    /// 版本(Major|Minor)
    /// </summary>
    public uint Version;
    /// <summary>
    /// 编辑标志,当这个整数为1时，表示该文件正在被编辑
    /// </summary>
    public uint EditFlag;
    /// <summary>
    /// Hash表在文件中的偏移
    /// </summary>
    public uint HashTableOffset;
    /// <summary>
    /// Block表在文件中的偏移
    /// </summary>
    public uint BlockTableOffset;
    /// <summary>
    /// Block表内容的个数
    /// </summary>
    public uint BlockTableCount;
    /// <summary>
    /// Block表最大容量大小(bytes)
    /// </summary>
    public uint BlockTableMaxSize;
    /// <summary>
    /// 数据块在文件中的偏移
    /// </summary>
    public uint DataOffset;
    /// <summary>
    /// 数据块的大小,包括空洞(bytes)
    /// </summary>
    public uint DataSize;
    /// <summary>
    /// 其中空洞数据块的大小(bytes)
    /// </summary>
    public uint DataHoleSize;
}
