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
        var readLength = 0;
        int rCount;
        while (readLength < limit)
        {
            rCount = Math.Min(0x1000, (int)limit - readLength);
            readLength += await stream.ReadAsync(textData, readLength, rCount);
        }
        var fileContent = _textEncoding.GetString(textData);
        //
        List<DbcFieldType> fieldTypes = new();
        List<string>? fieldNames;
        SortedDictionary<int, List<DbcField>> dataMap = new();
        using (var reader = new StringReader(fileContent))
        {
            await LoadTextFieldTypesAsync(reader, fieldTypes);
            fieldNames = await LoadTextFieldNamesAsync(reader, fieldTypes.Count);
            await LoadTextRowsAsync(reader, fieldTypes, dataMap);
        }
        return new(fieldTypes, fieldNames, dataMap);
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
            if (string.IsNullOrEmpty(str))
            {
                continue;
            }
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
        if (fieldTypes.Count == 0)
        {
            throw new Exception("未读取到有效的字段类型");
        }
    }

    private static async Task<List<string>?> LoadTextFieldNamesAsync(StringReader reader, int fieldCount)
    {
        var lineContent = await reader.ReadLineAsync();
        if (lineContent is null)
        {
            throw new Exception("read field names failed");
        }
        if (string.IsNullOrEmpty(lineContent))
        {
            return null;
        }
        var fieldNames = new List<string>(fieldCount);
        var strItems = lineContent.Split('\t');
        for (var i = 0; i < fieldCount; i++)
        {
            if (i < strItems.Length)
            {
                fieldNames.Add(strItems[i]);
            }
            else
            {
                fieldNames.Add(string.Empty);
            }
        }
        return fieldNames;
    }

    private static async Task LoadTextRowsAsync(StringReader reader, List<DbcFieldType> fieldTypes, SortedDictionary<int, List<DbcField>> dataMap)
    {
        string? lineContent;
        //int lineNumber = 2;
        while (true)
        {
            //lineNumber++;
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
            if (string.IsNullOrEmpty(strItems[0]))
            {
                continue;
            }
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
                        int intValue = 0;
                        if (!string.IsNullOrEmpty(fieldStr))
                        {
                            try
                            {
                                intValue = int.Parse(fieldStr);
                            }
                            catch (Exception)
                            {
                                //throw new Exception($"parse int value \"{fieldStr}\" failed at line {lineNumber} column {i}, {ex.Message}");
                            }
                        }
                        row.Add(new(intValue));
                        break;
                    case DbcFieldType.T_FLOAT:
                        float floatValue = 0;
                        if (!string.IsNullOrEmpty(fieldStr))
                        {
                            try
                            {
                                floatValue = float.Parse(fieldStr);
                            }
                            catch (Exception)
                            {
                                //throw new Exception($"parse int value \"{fieldStr}\" failed at line {lineNumber} column {i}, {ex.Message}");
                            }
                        }
                        row.Add(new(floatValue));
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
