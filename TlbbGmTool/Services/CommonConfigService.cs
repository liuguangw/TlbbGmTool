using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TlbbGmTool.Services
{
    public static class CommonConfigService
    {
        private static string GetConfigFilePath()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDir, "config", "common.xml");
        }

        public static async Task<(Dictionary<int, string>, Dictionary<int, string>, Dictionary<int, string>)>
            LoadMenpaiAndAttrAsync()
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

            var menpaiDictionary = new Dictionary<int, string>();
            var attr1Dictionary = new Dictionary<int, string>();
            var attr2Dictionary = new Dictionary<int, string>();
            var parentElement = commonXml.Descendants("menpai").First();
            foreach (var itemElement in parentElement.Descendants("item"))
            {
                var itemValue = Convert.ToInt32(itemElement.Attribute("value")?.Value ?? "0");
                menpaiDictionary.Add(itemValue, itemElement.Value);
            }

            parentElement = commonXml.Descendants("attr1").First();
            foreach (var itemElement in parentElement.Descendants("item"))
            {
                var itemValue = Convert.ToInt32(itemElement.Attribute("value")?.Value ?? "0");
                attr1Dictionary.Add(itemValue, itemElement.Value);
            }

            parentElement = commonXml.Descendants("attr2").First();
            foreach (var itemElement in parentElement.Descendants("item"))
            {
                var itemValue = Convert.ToInt32(itemElement.Attribute("value")?.Value ?? "0");
                attr2Dictionary.Add(itemValue, itemElement.Value);
            }

            return (menpaiDictionary, attr1Dictionary, attr2Dictionary);
        }
    }
}