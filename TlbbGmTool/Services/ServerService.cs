using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TlbbGmTool.Models;

namespace TlbbGmTool.Services
{
    public class ServerService
    {
        public static async Task<IEnumerable<GameServer>> LoadGameServers(string configPath)
        {
            if (!File.Exists(configPath))
            {
                throw new Exception($"配置文件{configPath}不存在");
            }

            string fileContent;
            using (var streamReader = File.OpenText(configPath))
            {
                try
                {
                    fileContent = await streamReader.ReadToEndAsync();
                }
                catch (Exception e)
                {
                    throw new Exception($"读取配置文件{configPath}出错,{e.Message}");
                }
            }

            XElement serverXml;
            try
            {
                serverXml = XElement.Parse(fileContent);
            }
            catch (Exception e)
            {
                throw new Exception($"解析配置文件{configPath}出错,{e.Message}");
            }

            var serverList = from serverEl in
                    serverXml.Descendants("server")
                select new GameServer()
                {
                    ServerName = (serverEl.Attribute("name")?.Value) ?? string.Empty,
                    DbHost = (serverEl.Element("host")?.Value) ?? string.Empty,
                    DbPort = (serverEl.Element("port")?.Value) ?? string.Empty,
                    AccountDbName = (serverEl.Element("accountDb")?.Value) ?? string.Empty,
                    GameDbName = (serverEl.Element("gameDb")?.Value) ?? string.Empty,
                    DbUser = (serverEl.Element("user")?.Value) ?? string.Empty,
                    DbPassword = (serverEl.Element("password")?.Value) ?? string.Empty,
                };
            return serverList;
        }
    }
}