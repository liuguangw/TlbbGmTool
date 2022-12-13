using Google.Protobuf.WellKnownTypes;
using liuguang.TlbbGmTool.ViewModels.Data;
using System;

namespace liuguang.TlbbGmTool.Services;
public static class EquipDataService
{
    /// <summary>
    /// 读取数据,放入equipData中
    /// </summary>
    /// <param name="itemBaseId"></param>
    /// <param name="pData"></param>
    /// <param name="equipData"></param>
    public static void Read(int itemBaseId, byte[] pData, EquipDataViewModel equipData)
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
            var value = ReadUshort(pData, offset);
            offset += 2;
            return value;
        };
        var readNextInt = () =>
        {
            var value = ReadInt(pData, offset);
            offset += 4;
            return value;
        };
        equipData.RulerId = readNextByte();
        //跳过固定为0的字节
        offset++;
        //前3个宝石
        equipData.Gem0 = readNextInt();
        equipData.Gem1 = readNextInt();
        equipData.Gem2 = readNextInt();
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
        //最后一个宝石
        offset += 16;
        equipData.Gem3 = readNextInt();
    }
    private static ushort ReadUshort(byte[] src, int startIndex)
    {
        if (BitConverter.IsLittleEndian)
        {
            return BitConverter.ToUInt16(src, startIndex);
        }
        var buff = new byte[2];
        Array.Copy(src, startIndex, buff, 0, buff.Length);
        Array.Reverse(buff);
        return BitConverter.ToUInt16(buff, 0);
    }
    private static int ReadInt(byte[] src, int startIndex)
    {
        if (BitConverter.IsLittleEndian)
        {
            return BitConverter.ToInt32(src, startIndex);
        }
        var buff = new byte[4];
        Array.Copy(src, startIndex, buff, 0, buff.Length);
        Array.Reverse(buff);
        return BitConverter.ToInt32(buff, 0);
    }
    /// <summary>
    /// 将数据写入到pData中
    /// </summary>
    /// <param name="equipData"></param>
    /// <param name="pData">17*4长度的字节数组</param>
    public static void Write(EquipDataViewModel equipData, byte[] pData)
    {
        int offset = 0;
        var writeNextByte = (byte value) =>
        {
            pData[offset] = value;
            offset++;
        };
        var writeNextUshort = (ushort value) =>
        {
            WriteData(pData, offset, value);
            offset += 2;
        };
        var writeNextInt = (int value) =>
        {
            WriteData(pData, offset, value);
            offset += 4;
        };
        //重新计算属性条数和嵌入的宝石个数
        equipData.ReloadStoneCount();
        equipData.ReloadAttrCount();
        //
        writeNextByte(equipData.RulerId);
        //跳过固定为0的字节
        offset++;
        //前3个宝石
        writeNextInt(equipData.Gem0);
        writeNextInt(equipData.Gem1);
        writeNextInt(equipData.Gem2);
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
        //最后一个宝石
        offset += 16;
        writeNextInt(equipData.Gem3);
    }
    /// <summary>
    /// 将字节数组转化为int数组(17个)
    /// </summary>
    /// <param name="pData"></param>
    /// <returns></returns>
    public static int[] ConvertToPArray(byte[] pData)
    {
        var pArray = new int[17];
        for (var i = 0; i < pArray.Length; i++)
        {
            pArray[i] = ReadInt(pData, i * 4);
        }
        return pArray;
    }
    private static void WriteData(byte[] destArray, int destIndex, ushort value)
    {
        byte[] buff = BitConverter.GetBytes(value);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(buff);
        }
        Array.Copy(buff, 0, destArray, destIndex, buff.Length);
    }
    private static void WriteData(byte[] destArray, int destIndex, int value)
    {
        byte[] buff = BitConverter.GetBytes(value);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(buff);
        }
        Array.Copy(buff, 0, destArray, destIndex, buff.Length);
    }
}
