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
    public static void Read(int itemBaseId, byte[] pData, GemDataViewModel gemData)
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
            //offset += 2;
            return value;
        };
        gemData.RulerId = readNextByte();
        //跳过固定为0的字节
        offset++;
        gemData.BasePrice = readNextUInt();
        gemData.AttrType = readNextByte();
        gemData.AttrValue = readNextUshort();
    }
    /// <summary>
    /// 将数据写入到pData中
    /// </summary>
    /// <param name="gemData"></param>
    /// <param name="pData">17*4长度的字节数组</param>
    public static void Write(GemDataViewModel gemData, byte[] pData)
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
            //offset += 2;
        };
        //
        writeNextByte(gemData.RulerId);
        //跳过固定为0的字节
        offset++;
        writeNextUInt(gemData.BasePrice);
        writeNextByte(gemData.AttrType);
        writeNextUshort(gemData.AttrValue);
    }
}
