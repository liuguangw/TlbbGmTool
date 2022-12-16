namespace liuguang.TlbbGmTool.Models;
public class ItemBaseGem : ItemBase
{
    public readonly uint BasePrice;
    public readonly byte AttrType;
    public readonly ushort AttrValue;

    public ItemBaseGem(int id, int tClass, int tType, byte rulerId,
        string name, string shortTypeString, string description, byte level,
        uint basePrice, byte attrType, ushort attrValue) : base(id, tClass, tType, rulerId, name, shortTypeString, description, level, 1)
    {
        BasePrice = basePrice;
        AttrType = attrType;
        AttrValue = attrValue;
    }
}
