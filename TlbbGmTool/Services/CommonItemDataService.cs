using liuguang.TlbbGmTool.ViewModels.Data;

namespace liuguang.TlbbGmTool.Services;
public static class CommonItemDataService
{
    /// <summary>
    /// 读取数据,放入itemData中
    /// </summary>
    /// <param name="itemBaseId"></param>
    /// <param name="pData"></param>
    /// <param name="itemData"></param>
    public static void Read(int itemBaseId, byte[] pData, CommonItemDataViewModel itemData)
    {
        itemData.ItemBaseId = itemBaseId;
        int offset = 0;
        var readNextByte = () =>
        {
            var value = pData[offset];
            offset++;
            return value;
        };
        var readNextInt = () =>
        {
            var value = DataService.ReadInt(pData, offset);
            offset += 4;
            return value;
        };
        var readNextUInt = () =>
        {
            var value = DataService.ReadUInt(pData, offset);
            offset += 4;
            return value;
        };
        itemData.RulerId = readNextByte();
        //跳过固定为0的字节
        offset++;
        var costSelf = readNextInt();
        itemData.CosSelf = (costSelf == 1);
        itemData.BasePrice = readNextUInt();
        itemData.MaxSize = readNextByte();
        itemData.Level = readNextByte();
        itemData.ReqSkill = readNextInt();
        itemData.ReqSkillLevel = readNextByte();
        itemData.ScriptID = readNextInt();
        itemData.SkillID = readNextInt();
        itemData.TargetType = readNextByte();
        itemData.BindStatus = readNextByte();
        itemData.Count = readNextByte();
        for (var i = 0; i < itemData.ItemParams.Length; i++)
        {
            itemData.ItemParams[i] = readNextByte();
        }
    }
    /// <summary>
    /// 将数据写入到pData中
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="pData">17*4长度的字节数组</param>
    public static void Write(CommonItemDataViewModel itemData, byte[] pData)
    {
        int offset = 0;
        var writeNextByte = (byte value) =>
        {
            pData[offset] = value;
            offset++;
        };
        var writeNextInt = (int value) =>
        {
            DataService.WriteData(pData, offset, value);
            offset += 4;
        };
        var writeNextUInt = (uint value) =>
        {
            DataService.WriteData(pData, offset, value);
            offset += 4;
        };
        //
        writeNextByte(itemData.RulerId);
        //跳过固定为0的字节
        offset++;
        int costSelf = itemData.CosSelf ? 1 : 0;
        writeNextInt(costSelf);
        writeNextUInt(itemData.BasePrice);
        writeNextByte(itemData.MaxSize);
        writeNextByte(itemData.Level);
        writeNextInt(itemData.ReqSkill);
        writeNextByte(itemData.ReqSkillLevel);
        writeNextInt(itemData.ScriptID);
        writeNextInt(itemData.SkillID);
        writeNextByte(itemData.TargetType);
        writeNextByte(itemData.BindStatus);
        writeNextByte(itemData.Count);
        for (var i = 0; i < itemData.ItemParams.Length; i++)
        {
            writeNextByte(itemData.ItemParams[i]);
        }
    }
}
