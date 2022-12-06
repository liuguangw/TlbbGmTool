namespace liuguang.TlbbGmTool.Models;

/// <summary>
/// 从txt文件中获取的心法基本信息
/// </summary>
public class XinFaBase
{
    /// <summary>
    /// 心法ID
    /// </summary>
    public readonly int Id;

    /// <summary>
    /// 门派ID
    /// </summary>
    public readonly int Menpai;

    /// <summary>
    /// 名称
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// 描述
    /// </summary>
    public readonly string Description;

    public XinFaBase(int id, int menpai, string name, string description)
    {
        Id = id;
        Menpai = menpai;
        Name = name;
        Description = description;
    }
}