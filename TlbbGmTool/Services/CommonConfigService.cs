using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace liuguang.TlbbGmTool.Services;

/// <summary>
/// 加载common.xml配置的工具
/// </summary>
public static class CommonConfigService
{
    private static string GetConfigFilePath()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        return Path.Combine(baseDir, "config", "common.xml");
    }

    /// <summary>
    /// 加载门派名称配置和攻击、防御属性名称配置
    /// </summary>
    /// <param name="menpaiMap"></param>
    /// <param name="attr1Map"></param>
    /// <param name="attr2Map"></param>
    /// <returns></returns>
    public static async Task LoadConfigAsync(SortedDictionary<int, string> menpaiMap, SortedDictionary<int, string> attr1Map, SortedDictionary<int, string> attr2Map)
    {
        var configFilePath = GetConfigFilePath();
        if (!File.Exists(configFilePath))
        {
            throw new Exception($"配置文件{configFilePath}不存在");
        }

        string fileContent;
        using (var streamReader = File.OpenText(configFilePath))
        {
            try
            {
                fileContent = await streamReader.ReadToEndAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"读取配置文件{configFilePath}出错,{e.Message}");
            }
        }

        XElement commonXml;
        try
        {
            commonXml = XElement.Parse(fileContent);
        }
        catch (Exception e)
        {
            throw new Exception($"解析配置文件{configFilePath}出错,{e.Message}");
        }
        LoadXmlItems(commonXml, "menpai", menpaiMap);
        LoadXmlItems(commonXml, "attr1", attr1Map);
        LoadXmlItems(commonXml, "attr2", attr2Map);
    }

    private static void LoadXmlItems(XElement commonXml, string itemTag, SortedDictionary<int, string> nameMap)
    {
        var parentElement = commonXml.Descendants(itemTag).First();
        if (parentElement is null)
        {
            return;
        }
        int itemValue;
        foreach (var itemElement in parentElement.Descendants("item"))
        {
            itemValue = Convert.ToInt32(itemElement.Attribute("value")?.Value ?? "0");
            nameMap.Add(itemValue, itemElement.Value);
        }
    }
}
