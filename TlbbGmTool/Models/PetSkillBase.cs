namespace liuguang.TlbbGmTool.Models;
public class PetSkillBase
{
    /// <summary>
    /// 技能ID
    /// </summary>
    public readonly int Id;
    /// <summary>
    /// 
    /// </summary>
    public readonly int SkillType;
    /// <summary>
    /// 名称
    /// </summary>
    public readonly string Name;
    /// <summary>
    /// 描述
    /// </summary>
    public readonly string Description;

    public PetSkillBase(int id, int skillType, string name, string description)
    {
        Id = id;
        SkillType = skillType;
        Name = name;
        Description = description;
    }
}
