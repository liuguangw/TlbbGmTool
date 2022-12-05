using System.Collections.Generic;

namespace TlbbGmTool.Models
{
    public class ItemBase
    {
        public int Id { get; }
        public int ItemClass { get; }
        public int ItemType { get; }

        public string Name { get; }
        public string ShortTypeString { get; }
        public string Description { get; }

        public int MaxSize { get; } = 1;
        public int Level { get; }

        //equip extra

        public int EquipPoint { get; }
        public int BagCapacity { get; }
        public int MaterialCapacity { get; }
        public int EquipVisual { get; }
        public int RuleId { get; }
        public int MaxLife { get; }

        //
        public string EquipTitle => $"({Level}级){Name} (ID: {Id})";


        /// <summary>
        /// for Equip
        /// </summary>
        public ItemBase(int id, int itemClass, int itemType,
            string name, string shortTypeString, string description, int level,
            int equipPoint, int bagCapacity, int materialCapacity,
            int equipVisual, int ruleId, int maxLife):this(id,itemClass,itemType,name,shortTypeString,description,level,1)
        {
            EquipPoint = equipPoint;
            BagCapacity = bagCapacity;
            MaterialCapacity = materialCapacity;
            EquipVisual = equipVisual;
            RuleId = ruleId;
            MaxLife = maxLife;
        }

        /// <summary>
        /// for CommonItem/GemInfo
        /// </summary>
        public ItemBase(int id, int itemClass, int itemType,
            string name, string shortTypeString, string description, int level, int maxSize)
        {
            Id = id;
            ItemClass = itemClass;
            ItemType = itemType;

            Name = name;
            ShortTypeString = shortTypeString;
            Description = description;
            Level = level;
            MaxSize = maxSize;
        }
    }
}