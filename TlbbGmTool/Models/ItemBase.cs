namespace liuguang.TlbbGmTool.Models;

/// <summary>
/// 从txt中读取的物品基本信息定义
/// </summary>
public class ItemBase
{
    public readonly int Id;
    public readonly int TClass;
    public readonly int TType;
    public readonly byte RulerId;

    public readonly string Name;
    public readonly string ShortTypeString;
    public readonly string Description;
    public readonly byte Level;
    public readonly byte MaxSize;

    public ItemBase(int id, int tClass, int tType, byte rulerId,
        string name, string shortTypeString, string description, byte level, byte maxSize)
    {
        Id = id;
        TClass = tClass;
        TType = tType;
        RulerId = rulerId;

        Name = name;
        ShortTypeString = shortTypeString;
        Description = description;
        Level = level;
        MaxSize = maxSize;
    }
}
