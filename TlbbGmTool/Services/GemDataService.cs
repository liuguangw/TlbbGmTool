using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.ViewModels.Data;

namespace liuguang.TlbbGmTool.Services;
public static class GemDataService
{
    /// <summary>
    /// 读取数据,放入gemData中
    /// </summary>
    /// <param name="itemBaseId"></param>
    /// <param name="pData"></param>
    /// <param name="gemData"></param>
    /// <param name="serverType">端类型</param>
    public static void Read(int itemBaseId, byte[] pData, GemDataViewModel gemData, ServerType serverType)
    {
        gemData.ItemBaseId = itemBaseId;
        int offset = 0;
        var readNextByte = () =>
        {
            var value = pData[offset];
            offset++;
            return value;
        };
        var readNextUInt = () =>
        {
            var value = DataService.ReadUInt(pData, offset);
            offset += 4;
            return value;
        };
        var readNextUshort = () =>
        {
            var value = DataService.ReadUshort(pData, offset);
            offset += 2;
            return value;
        };
        gemData.RulerId = readNextByte();
        if (serverType == ServerType.Common)
        {
            //跳过固定为0的字节
            offset++;
        }
        gemData.BasePrice = readNextUInt();
        gemData.AttrType = readNextByte();
        if (serverType == ServerType.HuaiJiu)
        {
            //跳过固定为0的字节
            offset++;
        }
        gemData.AttrValue = readNextUshort();
        if (serverType == ServerType.HuaiJiu)
        {
            //跳过2字节
            offset += 2;
            gemData.Count = readNextByte();
        }
        else
        {
            gemData.Count = 1;
        }
    }
    /// <summary>
    /// 将数据写入到pData中
    /// </summary>
    /// <param name="gemData"></param>
    /// <param name="pData">17*4长度的字节数组</param>
    /// <param name="serverType">端类型</param>
    /// <param name="serverType">端类型</param>
    public static void Write(GemDataViewModel gemData, byte[] pData, ServerType serverType)
    {
        int offset = 0;
        var writeNextByte = (byte value) =>
        {
            pData[offset] = value;
            offset++;
        };
        var writeNextUInt = (uint value) =>
        {
            DataService.WriteData(pData, offset, value);
            offset += 4;
        };
        var writeNextUshort = (ushort value) =>
        {
            DataService.WriteData(pData, offset, value);
            offset += 2;
        };
        //
        writeNextByte(gemData.RulerId);
        if (serverType == ServerType.Common)
        {
            //跳过固定为0的字节
            offset++;
        }
        writeNextUInt(gemData.BasePrice);
        writeNextByte(gemData.AttrType);
        if (serverType == ServerType.HuaiJiu)
        {
            //跳过固定为0的字节
            offset++;
        }
        writeNextUshort(gemData.AttrValue);
        if (serverType == ServerType.HuaiJiu)
        {
            ushort extraData = 0x01FA;
            writeNextUshort(extraData);
            writeNextByte(gemData.Count);
        }
    }
}
