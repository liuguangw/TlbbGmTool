using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.ViewModels.Data;

namespace liuguang.TlbbGmTool.Services;
public static class EquipDataService
{
    /// <summary>
    /// 读取数据,放入equipData中
    /// </summary>
    /// <param name="itemBaseId"></param>
    /// <param name="pData"></param>
    /// <param name="equipData"></param>
    /// <param name="serverType">端类型</param>
    public static void Read(int itemBaseId, byte[] pData, EquipDataViewModel equipData, ServerType serverType)
    {
        equipData.ItemBaseId = itemBaseId;
        int offset = 0;
        var readNextByte = () =>
        {
            var value = pData[offset];
            offset++;
            return value;
        };
        var readNextUshort = () =>
        {
            var value = DataService.ReadUshort(pData, offset);
            offset += 2;
            return value;
        };
        var readNextInt = () =>
        {
            var value = DataService.ReadInt(pData, offset);
            offset += 4;
            return value;
        };
        equipData.RulerId = readNextByte();
        if (serverType == ServerType.Common)
        {
            //跳过固定为0的字节
            offset++;
        }
        //前3个宝石
        equipData.Gem0 = readNextInt();
        equipData.Gem1 = readNextInt();
        equipData.Gem2 = readNextInt();
        if (serverType == ServerType.HuaiJiu)
        {
            equipData.Gem3 = readNextInt();
        }
        //
        equipData.BindStatus = readNextByte();
        equipData.MaxDurPoint = readNextByte();
        equipData.GemMaxCount = readNextByte();
        equipData.FaileTimes = readNextByte();
        equipData.CurDurPoint = readNextByte();
        equipData.CurDamagePoint = readNextUshort();
        equipData.AttrCount = readNextByte();
        equipData.StoneCount = readNextByte();
        equipData.EnhanceLevel = readNextByte();
        equipData.AdMagic = readNextByte();
        equipData.AdMagicValue = readNextUshort();
        //6个资质
        equipData.Aptitude0 = readNextByte();
        equipData.Aptitude1 = readNextByte();
        equipData.Aptitude2 = readNextByte();
        equipData.Aptitude3 = readNextByte();
        equipData.Aptitude4 = readNextByte();
        equipData.Aptitude5 = readNextByte();
        //
        equipData.StarCount = readNextByte();
        equipData.VisualId = readNextUshort();
        //属性条类型
        equipData.Attr0 = readNextInt();
        equipData.Attr1 = readNextInt();
        equipData.HiddenValue = readNextByte();
        if (serverType == ServerType.Common)
        {
            //最后一个宝石
            offset += 16;
            equipData.Gem3 = readNextInt();
        }
        equipData.DarkFlag = readNextByte();
    }

    /// <summary>
    /// 将数据写入到pData中
    /// </summary>
    /// <param name="equipData"></param>
    /// <param name="pData">17*4长度的字节数组</param>
    /// <param name="serverType">端类型</param>
    public static void Write(EquipDataViewModel equipData, byte[] pData, ServerType serverType)
    {
        int offset = 0;
        var writeNextByte = (byte value) =>
        {
            pData[offset] = value;
            offset++;
        };
        var writeNextUshort = (ushort value) =>
        {
            DataService.WriteData(pData, offset, value);
            offset += 2;
        };
        var writeNextInt = (int value) =>
        {
            DataService.WriteData(pData, offset, value);
            offset += 4;
        };
        //重新计算属性条数和嵌入的宝石个数
        equipData.ReloadStoneCount();
        equipData.ReloadAttrCount();
        //
        writeNextByte(equipData.RulerId);
        if (serverType == ServerType.Common)
        {
            //跳过固定为0的字节
            offset++;
        }
        //前3个宝石
        writeNextInt(equipData.Gem0);
        writeNextInt(equipData.Gem1);
        writeNextInt(equipData.Gem2);
        if (serverType == ServerType.HuaiJiu)
        {
            writeNextInt(equipData.Gem3);
        }
        //
        writeNextByte(equipData.BindStatus);
        writeNextByte(equipData.MaxDurPoint);
        writeNextByte(equipData.GemMaxCount);
        writeNextByte(equipData.FaileTimes);
        writeNextByte(equipData.CurDurPoint);
        writeNextUshort(equipData.CurDamagePoint);
        writeNextByte(equipData.AttrCount);
        writeNextByte(equipData.StoneCount);
        writeNextByte(equipData.EnhanceLevel);
        writeNextByte(equipData.AdMagic);
        writeNextUshort(equipData.AdMagicValue);
        //6个资质
        writeNextByte(equipData.Aptitude0);
        writeNextByte(equipData.Aptitude1);
        writeNextByte(equipData.Aptitude2);
        writeNextByte(equipData.Aptitude3);
        writeNextByte(equipData.Aptitude4);
        writeNextByte(equipData.Aptitude5);
        //
        writeNextByte(equipData.StarCount);
        writeNextUshort(equipData.VisualId);
        //属性条类型
        writeNextInt(equipData.Attr0);
        writeNextInt(equipData.Attr1);
        writeNextByte(equipData.HiddenValue);
        if (serverType == ServerType.Common)
        {
            //最后一个宝石
            offset += 16;
            writeNextInt(equipData.Gem3);
        }
        writeNextByte(equipData.DarkFlag);
    }
}
