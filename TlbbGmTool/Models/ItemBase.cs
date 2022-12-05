namespace liuguang.TlbbGmTool.Models;

public class ItemBase
{
    public readonly int Id;
    public readonly int TClass;
    public readonly int TType;

    public readonly string Name;
    public readonly string ShortTypeString;
    public readonly string Description;
    public readonly int Level;
    public readonly int MaxSize;

    //equip extra
    public readonly int EquipPoint;
    public readonly int BagCapacity;
    public readonly int MaterialCapacity;
    public readonly int EquipVisual;
    public readonly int MaxLife;
    public readonly int[]? EquipAttrValues;

    public ItemBase(int id, int tClass, int tType,
        string name, string shortTypeString, string description, int level, int maxSize)
    {
        Id = id;
        TClass = tClass;
        TType = tType;

        Name = name;
        ShortTypeString = shortTypeString;
        Description = description;
        Level = level;
        MaxSize = maxSize;
    }

    public ItemBase(int id, int tClass, int tType,
       string name, string shortTypeString, string description, int level,
       int equipPoint, int bagCapacity, int materialCapacity,
       int equipVisual, int maxLife, int[]? equipAttrValues) : this(id, tClass, tType, name, shortTypeString, description, level, 1)
    {
        EquipPoint = equipPoint;
        BagCapacity = bagCapacity;
        MaterialCapacity = materialCapacity;
        EquipVisual = equipVisual;
        MaxLife = maxLife;
        EquipAttrValues = equipAttrValues;
    }
}
