using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class SelectAttrViewModel : BindDataBase
    {
        #region Fields

        private SelectAttrWindow _selectAttrWindow;

        #endregion

        #region Properties

        public ObservableCollection<EquipAttributeNode> Attr1Selection { get; }
            = new ObservableCollection<EquipAttributeNode>();

        public ObservableCollection<EquipAttributeNode> Attr2Selection { get; }
            = new ObservableCollection<EquipAttributeNode>();

        public AppCommand ConfirmCommand { get; }

        #endregion

        public SelectAttrViewModel()
        {
            ConfirmCommand = new AppCommand(ConfirmSelect);
        }


        public void InitData(ItemBase equipBaseInfo, SelectAttrWindow selectAttrWindow,
            Dictionary<int, string> attr1CategoryList, Dictionary<int, string> attr2CategoryList,
            int attr1, int attr2)
        {
            _selectAttrWindow = selectAttrWindow;
            for (var i = 0; i < 32; i++)
            {
                    var attributeNode = new EquipAttributeNode(attr1CategoryList[i], i);
                    var attrIndexValue = attr1;
                    if (i > 0)
                    {
                        attrIndexValue >>= i;
                    }

                    attributeNode.AttributeChecked = (attrIndexValue & 1) != 0;
                    attributeNode.PropertyChanged += (sender, e) =>
                    {
                        if (e.PropertyName == nameof(attributeNode.AttributeChecked))
                        {
                            OnAttributeCheckChange();
                        }
                    };
                    Attr1Selection.Add(attributeNode);
                    //
                    attributeNode = new EquipAttributeNode(attr2CategoryList[i], i);
                    attrIndexValue = attr2;
                    if (i > 0)
                    {
                        attrIndexValue >>= i;
                    }

                    attributeNode.AttributeChecked = (attrIndexValue & 1) != 0;
                    attributeNode.PropertyChanged += (sender, e) =>
                    {
                        if (e.PropertyName == nameof(attributeNode.AttributeChecked))
                        {
                            OnAttributeCheckChange();
                        }
                    };
                    Attr2Selection.Add(attributeNode);
            }

            OnAttributeCheckChange();
        }

        private int GetSelectedAttrCount()
        {
            var attrCount = (from attributeNode in Attr1Selection
                where attributeNode.AttributeChecked
                select attributeNode).Count();
            attrCount += (from attributeNode in Attr2Selection
                where attributeNode.AttributeChecked
                select attributeNode).Count();
            return attrCount;
        }

        private void OnAttributeCheckChange()
        {
            const int maxAttrCount = 16;
            var attrCount = GetSelectedAttrCount();
            var enabledStatus = attrCount < maxAttrCount;
            foreach (var attributeNode in Attr1Selection)
            {
                attributeNode.AttributeEnabled =
                    attributeNode.AttributeChecked || enabledStatus;
            }

            foreach (var attributeNode in Attr2Selection)
            {
                attributeNode.AttributeEnabled =
                    attributeNode.AttributeChecked || enabledStatus;
            }
        }

        private void ConfirmSelect()
        {
            var attr1Value = 0;
            var attr2Value = 0;
            (from attributeNode in Attr1Selection
                    where attributeNode.AttributeChecked
                    select attributeNode)
                .ToList().ForEach(
                    attrNode => attr1Value |= 1 << attrNode.AttributeIndex
                );
            (from attributeNode in Attr2Selection
                    where attributeNode.AttributeChecked
                    select attributeNode)
                .ToList().ForEach(
                    attrNode => attr2Value |= 1 << attrNode.AttributeIndex
                );
            _selectAttrWindow.Attr1 = attr1Value;
            _selectAttrWindow.Attr2 = attr2Value;
            _selectAttrWindow.DialogResult = true;
            _selectAttrWindow.Close();
        }
    }
}