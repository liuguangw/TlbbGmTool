using TlbbGmTool.Core;

namespace TlbbGmTool.Models
{
    public class CommonItem : BindDataBase, ITextItem
    {
        #region Properties

        public int Id { get; }
        public int ItemClass { get; }
        public int ItemType { get; }

        public string Name { get; } = string.Empty;
        public string ShortTypeString { get; } = string.Empty;
        public string Description { get; } = string.Empty;
        public int MaxSize { get; } = 1;
        public int Level { get; } = 1;

        #endregion

        public CommonItem(int id, int itemClass, int itemType, string name,
            string shortTypeString, string description, int maxSize, int level)
        {
            Id = id;
            ItemClass = itemClass;
            ItemType = itemType;
            Name = name;
            ShortTypeString = shortTypeString;
            Description = description;
            MaxSize = maxSize;
            Level = level;
        }

        public int GetId() => Id;
    }
}