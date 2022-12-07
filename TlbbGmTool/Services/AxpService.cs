using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using liuguang.Axp;
using liuguang.TlbbGmTool.Models;
using System;
using liuguang.Dbc;
using System.Linq;

namespace liuguang.TlbbGmTool.Services;

public static class AxpService
{
    public static async Task LoadDataAsync(string axpFilePath, SortedDictionary<int, ItemBase> itemBaseMap, Dictionary<int, XinFaBase> xinFaMap)
    {
        using (var fileStream = File.OpenRead(axpFilePath))
        {
            var axpFile = await AxpFile.ReadAsync(fileStream);
            await LoadCommonItemAsync(fileStream, axpFile, itemBaseMap);
            await LoadGemInfoAsync(fileStream, axpFile, itemBaseMap);
            await LoadEquipBaseAsync(fileStream, axpFile, itemBaseMap);
            await LoadXinFaAsync(fileStream, axpFile, xinFaMap);
        }
    }

    private static async Task<DbcFile> ParseFileAsync(Stream stream, AxpFile axpFile, string filename)
    {
        var blockNode = axpFile.GetBlockNode(filename);
        if (!blockNode.HasValue)
        {
            throw new Exception("未找到文件" + filename);
        }
        stream.Seek(blockNode.Value.DataOffset, SeekOrigin.Begin);
        return await DbcFile.ReadAsync(stream, blockNode.Value.BlockSize);
    }

    private static async Task LoadCommonItemAsync(Stream stream, AxpFile axpFile, SortedDictionary<int, ItemBase> itemBaseMap)
    {
        var dbcFile = await ParseFileAsync(stream, axpFile, "CommonItem.txt");
        foreach (var keyValuePair in dbcFile.DataMap)
        {
            itemBaseMap[keyValuePair.Key] = ParseCommonItemRow(keyValuePair.Value);
        }
    }
    private static async Task LoadGemInfoAsync(Stream stream, AxpFile axpFile, SortedDictionary<int, ItemBase> itemBaseMap)
    {
        var dbcFile = await ParseFileAsync(stream, axpFile, "GemInfo.txt");
        foreach (var keyValuePair in dbcFile.DataMap)
        {
            itemBaseMap[keyValuePair.Key] = ParseGemInfoRow(keyValuePair.Value);
        }
    }

    private static async Task LoadEquipBaseAsync(Stream stream, AxpFile axpFile, SortedDictionary<int, ItemBase> itemBaseMap)
    {
        var dbcFile = await ParseFileAsync(stream, axpFile, "EquipBase.txt");
        var valueDbcFile = await ParseFileAsync(stream, axpFile, "ItemSegValue.txt");
        var segDictionary = ParseItemSegValue(valueDbcFile);
        foreach (var keyValuePair in dbcFile.DataMap)
        {
            itemBaseMap[keyValuePair.Key] = ParseEquipBaseRow(keyValuePair.Value, segDictionary);
        }
    }

    private static async Task LoadXinFaAsync(Stream stream, AxpFile axpFile, Dictionary<int, XinFaBase> xinFaMap)
    {
        var dbcFile = await ParseFileAsync(stream, axpFile, "XinFa_V1.txt");
        foreach (var keyValuePair in dbcFile.DataMap)
        {
            xinFaMap[keyValuePair.Key] = ParseXinFaRow(keyValuePair.Value);
        }
    }

    private static ItemBase ParseCommonItemRow(List<DbcField> rowFields)
    {
        var itemId = rowFields[0].IntValue;
        var tClass = rowFields[1].IntValue;
        var tType = rowFields[3].IntValue;
        //
        var name = rowFields[6].StringValue;
        var shortTypeString = rowFields[20].StringValue;
        var description = rowFields[7].StringValue;
        //
        var level = rowFields[8].IntValue;
        var maxSize = rowFields[12].IntValue;
        return new(itemId, tClass, tType, name, shortTypeString, description, level, maxSize);
    }
    private static ItemBase ParseGemInfoRow(List<DbcField> rowFields)
    {
        var itemId = rowFields[0].IntValue;
        var tClass = rowFields[1].IntValue;
        var tType = rowFields[3].IntValue;
        //
        var name = rowFields[7].StringValue;
        var shortTypeString = rowFields[76].StringValue;
        var description = rowFields[8].StringValue;
        //
        var level = rowFields[2].IntValue;
        var maxSize = 1;
        return new(itemId, tClass, tType, name, shortTypeString, description, level, maxSize);
    }

    private static Dictionary<int, int[]> ParseItemSegValue(DbcFile dbcFile)
    {
        var segValueDictionary = new Dictionary<int, int[]>(dbcFile.DataMap.Count);
        foreach (var keyPair in dbcFile.DataMap)
        {
            var rowFields = keyPair.Value;
            var itemId = rowFields[0].IntValue;
            var attrValues = new int[64];
            for (var i = 0; i < attrValues.Length; i++)
            {
                if (i + 1 < rowFields.Count)
                {
                    attrValues[i] = rowFields[i + 1].IntValue;
                }
            }
            segValueDictionary[itemId] = attrValues;
        }
        return segValueDictionary;
    }

    private static ItemBase ParseEquipBaseRow(List<DbcField> rowFields, Dictionary<int, int[]> segDictionary)
    {
        var itemId = rowFields[0].IntValue;
        var tClass = rowFields[1].IntValue;
        var tType = rowFields[3].IntValue;
        //
        var name = rowFields[10].StringValue;
        var shortTypeString = rowFields[22].StringValue;
        var description = rowFields[13].StringValue;
        //
        var level = rowFields[11].IntValue;
        //extra
        var equipPoint = rowFields[5].IntValue;
        var bagCapacity = rowFields[97].IntValue;
        var materialCapacity = rowFields[98].IntValue;
        var equipVisual = rowFields[6].IntValue;
        var maxLife = rowFields[16].IntValue;
        //起始数值段
        var segIndex = rowFields[91].IntValue;
        int[]? equipAttrValues = null;
        if (segIndex > 0)
        {
            if (segDictionary.TryGetValue(segIndex, out var segValues))
            {
                equipAttrValues = segValues;
            }
        }
        return new(itemId, tClass, tType, name, shortTypeString, description, level,
        equipPoint, bagCapacity, materialCapacity,
        equipVisual, maxLife, equipAttrValues);
    }

    private static XinFaBase ParseXinFaRow(List<DbcField> rowFields)
    {
        var id = rowFields[0].IntValue;
        var menpai = rowFields[1].IntValue;
        var name = rowFields[2].StringValue;
        var description = rowFields[3].StringValue;
        return new(id, menpai, name, description);
    }
}
