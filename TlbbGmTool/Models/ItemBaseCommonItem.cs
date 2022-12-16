namespace liuguang.TlbbGmTool.Models;
public class ItemBaseCommonItem : ItemBase
{
    /// <summary>
    /// 是否消耗自己
    /// </summary>
    public readonly bool CosSelf;
    public readonly uint BasePrice;
    public readonly int ReqSkill;
    public readonly byte ReqSkillLevel;
    public readonly int ScriptID;
    public readonly int SkillID;
    public readonly byte TargetType;

    public ItemBaseCommonItem(int id, int tClass, int tType, byte rulerId,
        string name, string shortTypeString, string description, byte level, byte maxSize,
        bool cosSelf, uint basePrice, int reqSkill, byte reqSkillLevel, int scriptID, int skillID, byte targetType) : base(id, tClass, tType, rulerId, name, shortTypeString, description, level, maxSize)
    {
        CosSelf = cosSelf;
        BasePrice = basePrice;
        ReqSkill = reqSkill;
        ReqSkillLevel = reqSkillLevel;
        ScriptID = scriptID;
        SkillID = skillID;
        TargetType = targetType;
    }
}
