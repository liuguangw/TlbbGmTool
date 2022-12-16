using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace liuguang.Dbc;
public partial class DbcFile
{
    const uint DBC_IDENTITY = 0xDDBBCC00;
    /// <summary>
    /// 文本编码
    /// </summary>
    private static Encoding _textEncoding = Encoding.GetEncoding("GB18030");
    /// <summary>
    /// 字段类型列表
    /// </summary>
    public List<DbcFieldType> FieldTypes;
    /// <summary>
    /// 字段名称列表
    /// </summary>
    public List<string>? FieldNames;
    /// <summary>
    /// 数据字典, [物品ID => 一行数据]
    /// </summary>
    public SortedDictionary<int, List<DbcField>> DataMap;

    private DbcFile(List<DbcFieldType> fieldTypes, SortedDictionary<int, List<DbcField>> dataMap)
    {
        FieldTypes = fieldTypes;
        DataMap = dataMap;
    }
    private DbcFile(List<DbcFieldType> fieldTypes, List<string>? fieldNames, SortedDictionary<int, List<DbcField>> dataMap) : this(fieldTypes, dataMap)
    {
        FieldNames = fieldNames;
    }
    private static async Task ReadBytesAsync(Stream stream, byte[] data)
    {
        var offset = 0;
        while (offset < data.Length)
        {
            var count = await stream.ReadAsync(data, offset, data.Length - offset);
            offset += count;
        }
    }


    public static async Task<DbcFile> ReadAsync(Stream stream, uint limit)
    {
        var data = new byte[4];
        await ReadBytesAsync(stream, data);
        var headFlag = BitConverter.ToUInt32(data, 0);
        if (headFlag == DBC_IDENTITY)
        {
            return await ReadBinaryAsync(stream);
        }
        stream.Seek(-4, SeekOrigin.Current);
        return await ReadTextAsync(stream, limit);
    }
}
