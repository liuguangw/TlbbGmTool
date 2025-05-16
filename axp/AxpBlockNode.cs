namespace liuguang.Axp;

public struct AxpBlockNode
{
    /// <summary>
    /// 对应的数据块在文件中的偏移
    /// </summary>
    public uint DataOffset;
    /// <summary>
    /// 数据块所对应的文件大小(bytes)
    /// </summary>
    public uint BlockSize;
    /// <summary>
    /// 数据块标志
    /// </summary>
    public uint Flags;
}
