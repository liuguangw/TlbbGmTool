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
        public List<int> Attr1;
        public List<int> Attr2;

        //
        public string EquipTitle => $"({Level}级){Name} (ID: {Id})";


        /// <summary>
        /// for Equip
        /// </summary>
        public ItemBase(int id, int itemClass, int itemType,
            string name, string shortTypeString, string description, int level,
            int equipPoint, int bagCapacity, int materialCapacity,
            int equipVisual, List<int> attr1, List<int> attr2)
        {
            Id = id;
            ItemClass = itemClass;
            ItemType = itemType;

            Name = name;
            ShortTypeString = shortTypeString;
            Description = description;
            Level = level;
            EquipPoint = equipPoint;
            BagCapacity = bagCapacity;
            MaterialCapacity = materialCapacity;
            EquipVisual = equipVisual;
            Attr1 = attr1;
            Attr2 = attr2;
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