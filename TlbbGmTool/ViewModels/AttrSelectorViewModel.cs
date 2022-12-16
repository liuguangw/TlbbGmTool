using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.ViewModels.Data;
using liuguang.TlbbGmTool.Views.Item;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace liuguang.TlbbGmTool.ViewModels;
public class AttrSelectorViewModel : ViewModelBase
{
    public List<EquipAttributeNode> Attr0Selection { get; } = new();
    public List<EquipAttributeNode> Attr1Selection { get; } = new();

    public int[] EquipValueAttrs
    {
        set
        {
            for (var i = 0; i < value.Length; i++)
            {
                var attrValue = value[i];
                if (attrValue <= 0)
                {
                    continue;
                }
                var attrIndex = i % 32;
                if (i < 32)
                {
                    LoadAttrSelection(0, attrIndex);
                }
                else
                {
                    LoadAttrSelection(1, attrIndex);
                }
            }
            RaisePropertyChanged(nameof(Attr0Selection));
            RaisePropertyChanged(nameof(Attr1Selection));
            WatchAttrCount();
        }
    }

    public int Attr0
    {
        set
        {
            InitAttr(0, value);
        }
    }
    public int Attr1
    {
        set
        {
            InitAttr(1, value);
        }
    }

    public Command ConfirmCommand { get; }

    public AttrSelectorViewModel()
    {
        ConfirmCommand = new(ConfirmSelect);
    }
    private void LoadAttrSelection(int selectionIndex, int attrIndex)
    {
        string attrName;
        if (selectionIndex == 0)
        {
            attrName = SharedData.Attr0Map[attrIndex];
        }
        else
        {
            attrName = SharedData.Attr1Map[attrIndex];
        }
        var attrNode = new EquipAttributeNode(attrName, attrIndex);

        if (selectionIndex == 0)
        {
            Attr0Selection.Add(attrNode);
        }
        else
        {
            Attr1Selection.Add(attrNode);
        }
    }
    private void InitAttr(int selectionIndex, int attrValue)
    {
        var markSelected = (int attrIndex) =>
        {
            var selection = (selectionIndex == 0) ? Attr0Selection : Attr1Selection;
            foreach (var attrNode in selection)
            {
                if (attrNode.Index == attrIndex)
                {
                    attrNode.Checked = true;
                }
            }
        };
        for (var i = 0; i < 32; i++)
        {
            var tmpValue = attrValue;
            if (i > 0)
            {
                tmpValue >>= i;
            }

            if ((tmpValue & 1) != 0)
            {
                markSelected(i);
            }
        }
    }

    private void ConfirmSelect()
    {
        if (OwnedWindow is not AttrSelectorWindow currentWindow)
        {
            return;
        }
        var calcAttrValue = (int selectionIndex) =>
        {
            var attrValue = 0;
            var selection = (selectionIndex == 0) ? Attr0Selection : Attr1Selection;
            foreach (var attrNode in selection)
            {
                if (attrNode.Checked)
                {
                    attrValue |= (1 << attrNode.Index);
                }
            }
            return attrValue;
        };
        currentWindow.Attr0 = calcAttrValue(0);
        currentWindow.Attr1 = calcAttrValue(1);
        currentWindow.DialogResult = true;
        currentWindow.Close();
    }

    private void WatchAttrCount()
    {
        foreach (var attrNode in Attr0Selection)
        {
            attrNode.PropertyChanged += AttrNode_PropertyChanged;
        }
        foreach (var attrNode in Attr1Selection)
        {
            attrNode.PropertyChanged += AttrNode_PropertyChanged;
        }
    }

    private void AttrNode_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is EquipAttributeNode attrNode)
        {
            if (e.PropertyName == nameof(attrNode.Checked))
            {
                OnAttributeCheckChange();
            }
        }
    }
    /// <summary>
    /// 计算已选择的属性条数
    /// </summary>
    /// <returns></returns>
    private int GetSelectedAttrCount()
    {
        var attrCount = (from attrNode in Attr0Selection
                         where attrNode.Checked
                         select attrNode).Count();
        attrCount += (from attrNode in Attr1Selection
                      where attrNode.Checked
                      select attrNode).Count();
        return attrCount;
    }
    /// <summary>
    /// 属性条数达到限制后,禁止选择剩余的属性
    /// </summary>
    private void OnAttributeCheckChange()
    {
        const int maxAttrCount = 16;
        var attrCount = GetSelectedAttrCount();
        var enabledStatus = attrCount < maxAttrCount;
        foreach (var attrNode in Attr0Selection)
        {
            attrNode.Enabled =
                attrNode.Checked || enabledStatus;
        }

        foreach (var attrNode in Attr1Selection)
        {
            attrNode.Enabled =
                attrNode.Checked || enabledStatus;
        }
    }
}
