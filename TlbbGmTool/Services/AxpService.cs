using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using liuguang.Axp;
using liuguang.TlbbGmTool.Models;
using System;
using liuguang.Dbc;
using liuguang.TlbbGmTool.Common;

namespace liuguang.TlbbGmTool.Services;

public static class AxpService
{
    public static async Task LoadDataAsync(string axpFilePath)
    {
        using (var fileStream = File.OpenRead(axpFilePath))
        {
            var axpFile = await AxpFile.ReadAsync(fileStream);
            await LoadCommonItemAsync(fileStream, axpFile, SharedData.ItemBaseMap);
            await LoadGemInfoAsync(fileStream, axpFile, SharedData.ItemBaseMap);
            await LoadEquipBaseAsync(fileStream, axpFile, SharedData.ItemBaseMap);
            await LoadXinFaAsync(fileStream, axpFile, SharedData.XinFaMap);
            await LoadPetSkillAsync(fileStream, axpFile, SharedData.PetSkillMap);
            await LoadDarkImpactAsync(fileStream, axpFile, SharedData.DarkImpactMap);
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
        DbcFile fileResult;
        try
        {
            fileResult = await DbcFile.ReadAsync(stream, blockNode.Value.BlockSize);
        }
        catch (Exception ex)
        {
            throw new Exception($"解析文件{filename}出错,{ex.Message}", ex);
        }
        return fileResult;
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

    private static async Task LoadPetSkillAsync(Stream stream, AxpFile axpFile, SortedDictionary<int, PetSkillBase> petSkillMap)
    {
        var dbcFile = await ParseFileAsync(stream, axpFile, "SkillTemplate_V1.txt");
        foreach (var keyValuePair in dbcFile.DataMap)
        {
            var rowFields = keyValuePair.Value;
            if (rowFields.Count >= 75)
            {
                var skillType = rowFields[27].IntValue;
                if (skillType >= 0)
                {
                    petSkillMap[keyValuePair.Key] = ParsePetSkillRow(skillType, rowFields);
                }
            }
        }
    }

    private static async Task LoadDarkImpactAsync(Stream stream, AxpFile axpFile, SortedDictionary<int, string> darkImpactMap)
    {
        var dbcFile = await ParseFileAsync(stream, axpFile, "DarkImpackStr.txt");
        foreach (var keyValuePair in dbcFile.DataMap)
        {
            darkImpactMap[keyValuePair.Key] = keyValuePair.Value[1].StringValue;
        }
    }

    private static ItemBaseCommonItem ParseCommonItemRow(List<DbcField> rowFields)
    {
        var itemId = rowFields[0].IntValue;
        var tClass = rowFields[1].IntValue;
        var tType = rowFields[3].IntValue;
        var rulerId = (byte)rowFields[11].IntValue;
        //
        var name = rowFields[6].StringValue;
        var shortTypeString = rowFields[20].StringValue;
        var description = rowFields[7].StringValue;
        //
        var level = (byte)rowFields[8].IntValue;
        var maxSize = (byte)rowFields[12].IntValue;
        //
        var cosSelf = (rowFields[15].IntValue == 1);
        var basePrice = (uint)rowFields[9].IntValue;
        var reqSkill = rowFields[16].IntValue;
        var reqSkillLevel = (byte)rowFields[17].IntValue;
        var scriptID = rowFields[13].IntValue;
        var skillID = rowFields[14].IntValue;
        var targetType = (byte)rowFields[19].IntValue;
        return new(itemId, tClass, tType, rulerId, name, shortTypeString, description, level, maxSize,
            cosSelf, basePrice, reqSkill, reqSkillLevel, scriptID, skillID, targetType);
    }
    private static ItemBaseGem ParseGemInfoRow(List<DbcField> rowFields)
    {
        var itemId = rowFields[0].IntValue;
        var tClass = rowFields[1].IntValue;
        var tType = rowFields[3].IntValue;
        var rulerId = (byte)rowFields[6].IntValue;
        //
        var name = rowFields[7].StringValue;
        var shortTypeString = rowFields[76].StringValue;
        var description = rowFields[8].StringValue;
        //
        var level = (byte)rowFields[2].IntValue;
        //
        var basePrice = (uint)rowFields[9].IntValue;
        byte attrType = 0;
        ushort attrValue = 0;
        var attrStartIndex = 11;
        var attrEndIndex = 11 + 64;
        for (var i = attrStartIndex; i < attrEndIndex; i++)
        {
            if (rowFields[i].IntValue > 0)
            {
                attrValue = (ushort)rowFields[i].IntValue;
                break;
            }
            attrType++;
        }
        return new(itemId, tClass, tType, rulerId, name, shortTypeString, description, level,
            basePrice, attrType, attrValue);
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

    private static ItemBaseEquip ParseEquipBaseRow(List<DbcField> rowFields, Dictionary<int, int[]> segDictionary)
    {
        var itemId = rowFields[0].IntValue;
        var tClass = rowFields[1].IntValue;
        var tType = rowFields[3].IntValue;
        var rulerId = (byte)rowFields[7].IntValue;
        //
        var name = rowFields[10].StringValue;
        var shortTypeString = rowFields[22].StringValue;
        var description = rowFields[13].StringValue;
        //
        var level = (byte)rowFields[11].IntValue;
        //extra
        var equipPoint = rowFields[5].IntValue;
        var bagCapacity = rowFields[97].IntValue;
        var materialCapacity = rowFields[98].IntValue;
        var equipVisual = (ushort)rowFields[6].IntValue;
        var maxDurPoint = (byte)rowFields[16].IntValue;
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
        return new(itemId, tClass, tType, rulerId, name, shortTypeString, description, level,
        equipPoint, bagCapacity, materialCapacity,
        equipVisual, maxDurPoint, equipAttrValues);
    }

    private static XinFaBase ParseXinFaRow(List<DbcField> rowFields)
    {
        var id = rowFields[0].IntValue;
        var menpai = rowFields[1].IntValue;
        var name = rowFields[2].StringValue;
        var description = rowFields[3].StringValue;
        return new(id, menpai, name, description);
    }

    private static PetSkillBase ParsePetSkillRow(int skillType, List<DbcField> rowFields)
    {
        var id = rowFields[0].IntValue;
        var name = rowFields[3].StringValue;
        var description = rowFields[74].StringValue;
        return new(id, skillType, name, description);
    }
}
