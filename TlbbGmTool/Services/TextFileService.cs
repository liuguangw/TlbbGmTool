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
            return Path.Combine(baseDir, "config", fileName);
        }

        public static async Task<Dictionary<int, PetSkill>> LoadPetSkillList()
        {
            var itemList = new Dictionary<int, PetSkill>();
            var textFilePath = GetTextFilePath("SkillTemplate_V1.txt");
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

                            var itemInfo = ParseSkillLine(lineContent);
                            if (itemInfo != null)
                            {
                                itemList.Add(itemInfo.Id, itemInfo);
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
    }
}