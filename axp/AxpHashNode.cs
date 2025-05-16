namespace liuguang.Axp;

public struct AxpHashNode
{
    /// <summary>
    /// 哈希值 A，用于校验
    /// </summary>
    public uint HashA;
    /// <summary>
    /// 哈希值 B，用于校验
    /// </summary>
    public uint HashB;
    /// <summary>
    /// 数据
    /// </summary>
    public uint Data;


    public bool Exists()
    {
        return (Data & 0x8000_0000) != 0;
    }

    public uint BlockIndex()
    {
        return Data & 0x3FFF_FFFF;
    }
}
