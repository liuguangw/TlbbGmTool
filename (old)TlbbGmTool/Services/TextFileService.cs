using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            return Path.Combine(baseDir, "config", "text", fileName);
        }


        public static async Task<List<PetSkill>> LoadPetSkillList()
        {
            return await LoadItemList("SkillTemplate_V1.txt", ParseSkillLine);
        }

        public static async Task<List<ItemBase>> LoadCommonItemList()
        {
            return await LoadItemList("CommonItem.txt", ParseCommonItemLine);
        }

        public static async Task<List<ItemBase>> LoadGemItemList()
        {
            return await LoadItemList("GemInfo.txt", ParseGemItemLine);
        }

        public static async Task<List<ItemBase>> LoadEquipBaseList()
        {
            return await LoadItemList("EquipBase.txt", ParseEquipBaseLine);
        }

        private static async Task<List<T>>
            LoadItemList<T>(string textFileName, Func<string, T> lineParser)
        {
            var itemList = new List<T>();
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
                                itemList.Add(itemInfo);
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

        private static ItemBase ParseCommonItemLine(string lineContent)
        {
            var columns = lineContent.Split('\t');
            const int minColumnSize = 26;
            if (columns.Length < minColumnSize)
            {
                throw new Exception("字段长度不足");
            }

            var itemId = Convert.ToInt32(columns[0]);
            var (itemClass, itemType) = GetItemClassAndType(itemId);
            var name = columns[6];
            var shortTypeString = columns[20];
            var description = columns[7];
            var maxSize = Convert.ToInt32(columns[12]);
            var level = Convert.ToInt32(columns[8]);
            return new ItemBase(itemId, itemClass, itemType, name, shortTypeString, description, level, maxSize);
        }

        private static ItemBase ParseGemItemLine(string lineContent)
        {
            var columns = lineContent.Split('\t');
            const int minColumnSize = 80;
            if (columns.Length < minColumnSize)
            {
                throw new Exception("字段长度不足");
            }

            var itemId = Convert.ToInt32(columns[0]);
            var (itemClass, itemType) = GetItemClassAndType(itemId);
            var name = columns[7];
            var shortTypeString = columns[76];
            var description = columns[8];
            var level = Convert.ToInt32(columns[2]);
            return new ItemBase(itemId, itemClass, itemType, name, shortTypeString, description, level, 1);
        }

        private static ItemBase ParseEquipBaseLine(string lineContent)
        {
            var columns = lineContent.Split('\t');
            const int minColumnSize = 100;
            if (columns.Length < minColumnSize)
            {
                throw new Exception("字段长度不足");
            }

            var itemId = Convert.ToInt32(columns[0]);
            var (itemClass, itemType) = GetItemClassAndType(itemId);
            var equipPoint = Convert.ToInt32(columns[5]);
            var name = columns[10];
            var shortTypeString = columns[22];
            var description = columns[13];
            var level = Convert.ToInt32(columns[11]);
            var bagCapacity = Convert.ToInt32(columns[97]);
            var materialCapacity = Convert.ToInt32(columns[98]);
            var equipVisual = Convert.ToInt32(columns[6]);
            var ruleId = Convert.ToInt32(columns[7]);
            var maxLife = Convert.ToInt32(columns[16]);
            return new ItemBase(itemId, itemClass, itemType,
                name, shortTypeString, description, level, equipPoint, bagCapacity, materialCapacity,
                equipVisual, ruleId, maxLife);
        }

        private static (int, int) GetItemClassAndType(int itemId)
        {
            var itemClass = itemId / 1000_0000;
            var itemType = itemId % 10_0000 / 1000;
            return (itemClass, itemType);
        }
    }
}