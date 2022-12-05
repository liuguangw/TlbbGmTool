using System;

namespace TlbbGmTool.Core
{
    public class EquipAttributeNode : BindDataBase
    {
        #region Fields

        private bool _attributeEnabled = true;
        private bool _attributeChecked = false;

        #endregion

        #region Properties

        public string Name { get; }
        public int AttributeIndex { get; }

        public bool AttributeChecked
        {
            get => _attributeChecked;
            set => SetProperty(ref _attributeChecked, value);
        }

        public bool AttributeEnabled
        {
            get => _attributeEnabled;
            set => SetProperty(ref _attributeEnabled, value);
        }

        #endregion

        public EquipAttributeNode(string name, int attributeIndex)
        {
            Name = name;
            AttributeIndex = attributeIndex;
        }
    }
}