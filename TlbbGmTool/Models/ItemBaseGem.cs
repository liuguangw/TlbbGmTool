namespace liuguang.TlbbGmTool.Models;
public class ItemBaseGem : ItemBase
{
    public readonly uint BasePrice;
    public readonly byte AttrType;
    public readonly ushort AttrValue;

    public ItemBaseGem(int id, int tClass, int tType, byte rulerId,
        string name, string shortTypeString, string description, byte level, byte maxSize,
        uint basePrice, byte attrType, ushort attrValue) : base(id, tClass, tType, rulerId, name, shortTypeString, description, level, maxSize)
    {
        BasePrice = basePrice;
        AttrType = attrType;
        AttrValue = attrValue;
    }
}
