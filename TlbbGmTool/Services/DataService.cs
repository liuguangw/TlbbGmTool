using System;

namespace liuguang.TlbbGmTool.Services;
/// <summary>
/// 处理数据读写、转换
/// </summary>
public static class DataService
{
    public static ushort ReadUshort(byte[] src, int startIndex)
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
    public static int ReadInt(byte[] src, int startIndex)
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
    public static uint ReadUInt(byte[] src, int startIndex)
    {
        if (BitConverter.IsLittleEndian)
        {
            return BitConverter.ToUInt32(src, startIndex);
        }
        var buff = new byte[4];
        Array.Copy(src, startIndex, buff, 0, buff.Length);
        Array.Reverse(buff);
        return BitConverter.ToUInt32(buff, 0);
    }

    public static void WriteData(byte[] destArray, int destIndex, ushort value)
    {
        byte[] buff = BitConverter.GetBytes(value);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(buff);
        }
        Array.Copy(buff, 0, destArray, destIndex, buff.Length);
    }
    public static void WriteData(byte[] destArray, int destIndex, int value)
    {
        byte[] buff = BitConverter.GetBytes(value);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(buff);
        }
        Array.Copy(buff, 0, destArray, destIndex, buff.Length);
    }
    public static void WriteData(byte[] destArray, int destIndex, uint value)
    {
        byte[] buff = BitConverter.GetBytes(value);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(buff);
        }
        Array.Copy(buff, 0, destArray, destIndex, buff.Length);
    }

    /// <summary>
    /// 将字节数组转化为int数组
    /// </summary>
    /// <param name="pData"></param>
    /// <returns></returns>
    public static int[] ConvertToPArray(byte[] pData)
    {
        var pArray = new int[pData.Length / 4];
        for (var i = 0; i < pArray.Length; i++)
        {
            pArray[i] = ReadInt(pData, i * 4);
        }
        return pArray;
    }

    /// <summary>
    /// 把int数组转化为字节数组
    /// </summary>
    /// <param name="pArray"></param>
    /// <returns></returns>
    public static byte[] ConvertToPData(int[] pArray)
    {
        var pData = new byte[pArray.Length * 4];
        for (var i = 0; i < pArray.Length; i++)
        {
            WriteData(pData, i * 4, pArray[i]);
        }
        return pData;
    }
}
