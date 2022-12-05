using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace liuguang.Dbc;

public partial class DbcFile
{
    private static async Task<DbcFile> ReadTextAsync(Stream stream, uint limit)
    {
        var textData = new byte[limit];
        var buff = new byte[0x1000];
        var readLength = 0;
        int rCount;
        while (readLength < limit)
        {
            rCount = Math.Min(buff.Length, (int)limit - readLength);
            readLength += await stream.ReadAsync(textData, readLength, rCount);
        }
        var fileContent = _textEncoding.GetString(textData);
        //
        List<DbcFieldType> fieldTypes = new();
        SortedDictionary<int, List<DbcField>> dataMap = new();
        using (var reader = new StringReader(fileContent))
        {
            await LoadTextFieldTypesAsync(reader, fieldTypes);
            await LoadTextRowsAsync(reader, fieldTypes, dataMap);
        }
        return new(fieldTypes, dataMap);
    }
    private static async Task LoadTextFieldTypesAsync(StringReader reader, List<DbcFieldType> fieldTypes)
    {
        var firstLine = await reader.ReadLineAsync();
        if (firstLine is null)
        {
            throw new Exception("读取dbc首行出错");
        }
        var fieldTypeStrs = firstLine.Split('\t');
        var comparisonType = StringComparison.OrdinalIgnoreCase;
        foreach (var str in fieldTypeStrs)
        {
            if (str.Equals("int", comparisonType))
            {
                fieldTypes.Add(DbcFieldType.T_INT);
            }
            else if (str.Equals("float", comparisonType))
            {
                fieldTypes.Add(DbcFieldType.T_FLOAT);
            }
            else if (str.Equals("string", comparisonType))
            {
                fieldTypes.Add(DbcFieldType.T_STRING);
            }
            else
            {
                throw new Exception("未知字段类型" + str);
            }
        }
    }

    private static async Task LoadTextRowsAsync(StringReader reader, List<DbcFieldType> fieldTypes, SortedDictionary<int, List<DbcField>> dataMap)
    {
        //空读一行
        await reader.ReadLineAsync();
        string? lineContent;
        while (true)
        {
            lineContent = await reader.ReadLineAsync();
            if (lineContent is null)
            {
                break;
            }
            if ((lineContent == string.Empty) || (lineContent.StartsWith("#")))
            {
                continue;
            }
            var strItems = lineContent.Split('\t');
            //读取ID
            int rowID = int.Parse(strItems[0]);
            var row = new List<DbcField>(fieldTypes.Count){
                new(rowID)
            };
            //读取剩余列
            for (int i = 1; i < fieldTypes.Count; i++)
            {
                var fieldType = fieldTypes[i];
                var fieldStr = (i < strItems.Length) ? strItems[i] : string.Empty;
                switch (fieldType)
                {
                    case DbcFieldType.T_INT:
                        row.Add(new(int.Parse(fieldStr)));
                        break;
                    case DbcFieldType.T_FLOAT:
                        row.Add(new(float.Parse(fieldStr)));
                        break;
                    case DbcFieldType.T_STRING:
                        row.Add(new(fieldStr));
                        break;
                }
            }
            dataMap[rowID] = row;
        }
    }
}
