using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TlbbGmTool.Core;
using TlbbGmTool.Models;

namespace TlbbGmTool.Services
{
    public static class TextFileService
    {
        private static readonly Encoding FileEncoding = Encoding.GetEncoding("GB18030");

        private static readonly Regex LineRegex = new Regex(@"^\d+\t", RegexOptions.Compiled);

        private static string GetTextFilePath(string fileName)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDir, "config", fileName);
        }


        public static async Task<Dictionary<int, PetSkill>> LoadPetSkillList()
        {
            return await LoadItemList("SkillTemplate_V1.txt", ParseSkillLine);
        }

        public static async Task<Dictionary<int, CommonItem>> LoadCommonItemList()
        {
            return await LoadItemList("CommonItem.txt", ParseCommonItemLine);
        }
        
        public static async Task<Dictionary<int, CommonItem>> LoadGemItemList()
        {
            return await LoadItemList("GemInfo.txt", ParseGemItemLine);
        }

        private static async Task<Dictionary<int, T>>
            LoadItemList<T>(string textFileName, Func<string, T> lineParser) where T : ITextItem
        {
            var itemList = new Dictionary<int, T>();
            var textFilePath = GetTextFilePath(textFileName);
            if (!File.Exists(textFilePath))
            {
                throw new Exception($"文件{textFilePath}不存在");
            }

            try
            {
                await Task.Run(async () =>
                {
                    using (var streamReader = new StreamReader(textFilePath, FileEncoding))
                    {
                        while (true)
                        {
                            var lineContent = await streamReader.ReadLineAsync();
                            if (lineContent == null)
                            {
                                break;
                            }

                            if (!LineRegex.IsMatch(lineContent))
                            {
                                continue;
                            }

                            var itemInfo = lineParser(lineContent);
                            if (itemInfo != null)
                            {
                                itemList.Add(itemInfo.GetId(), itemInfo);
                            }
                        }
                    }
                });
            }
            catch (Exception e)
            {
                throw new Exception($"读取文件{textFilePath}出错,{e.Message}");
            }

            return itemList;
        }

        private static PetSkill ParseSkillLine(string lineContent)
        {
            var columns = lineContent.Split('\t');
            const int minColumnSize = 28;
            if (columns.Length < minColumnSize)
            {
                throw new Exception("字段长度不足");
            }

            var skillId = Convert.ToInt32(columns[0]);
            var skillName = columns[3];
            //AB段
            var skillType = Convert.ToInt32(columns[27]);
            if (skillType != 0 && skillType != 1 && skillType != 2)
            {
                return null;
            }

            return new PetSkill(skillId, skillName, skillType);
        }

        private static CommonItem ParseCommonItemLine(string lineContent)
        {
            var columns = lineContent.Split('\t');
            const int minColumnSize = 26;
            if (columns.Length < minColumnSize)
            {
                throw new Exception("字段长度不足");
            }

            var itemId = Convert.ToInt32(columns[0]);
            var itemClass = Convert.ToInt32(columns[1]);
            var itemType = Convert.ToInt32(columns[3]);
            var name = columns[6];
            var shortTypeString = columns[20];
            var description = columns[7];
            var maxSize = Convert.ToInt32(columns[12]);
            var level = Convert.ToInt32(columns[8]);
            return new CommonItem(itemId, itemClass, itemType, name, shortTypeString, description, maxSize, level);
        }
        
        private static CommonItem ParseGemItemLine(string lineContent)
        {
            var columns = lineContent.Split('\t');
            const int minColumnSize = 80;
            if (columns.Length < minColumnSize)
            {
                throw new Exception("字段长度不足");
            }

            var itemId = Convert.ToInt32(columns[0]);
            var itemClass = Convert.ToInt32(columns[1]);
            var itemType = Convert.ToInt32(columns[3]);
            var name = columns[7];
            var shortTypeString = columns[76];
            var description = columns[8];
            var maxSize = 1;
            var level = Convert.ToInt32(columns[2]);
            return new CommonItem(itemId, itemClass, itemType, name, shortTypeString, description, maxSize, level);
        }
    }
}