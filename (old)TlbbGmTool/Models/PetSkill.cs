namespace TlbbGmTool.Models
{
    public class PetSkill
    {
        #region Properties

        public int Id { get; }

        public string Name { get; }

        /// <summary>
        /// 0主动 1自动 2buff
        /// </summary>
        public int SkillType { get; }

        #endregion

        public PetSkill(int id, string name, int skillType)
        {
            Id = id;
            Name = name;
            SkillType = skillType;
        }
    }
}