using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TlbbGmTool.Models;

namespace TlbbGmTool.Services
{
    public static class ServerService
    {
        private static string GetConfigFilePath()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDir, "config", "servers.xml");
        }

        public static async Task<IEnumerable<GameServer>> LoadGameServers()
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

            XElement serverXml;
            try
            {
                serverXml = XElement.Parse(fileContent);
            }
            catch (Exception e)
            {
                throw new Exception($"解析配置文件{configFilePath}出错,{e.Message}");
            }

            var serverList = from serverEl in
                    serverXml.Descendants("server")
                select new GameServer()
                {
                    ServerName = serverEl.Attribute("name")?.Value ?? string.Empty,
                    DbHost = serverEl.Element("host")?.Value ?? string.Empty,
                    DbPort = Convert.ToUInt32(serverEl.Element("port")?.Value ?? "3306"),
                    AccountDbName = serverEl.Element("accountDb")?.Value ?? string.Empty,
                    GameDbName = serverEl.Element("gameDb")?.Value ?? string.Empty,
                    DbUser = serverEl.Element("user")?.Value ?? string.Empty,
                    DbPassword = serverEl.Element("password")?.Value ?? string.Empty,
                };
            return serverList;
        }

        public static async Task SaveGameServers(IEnumerable<GameServer> gameServers)
        {
            var configFilePath = GetConfigFilePath();
            var xmlTree = new XElement("servers");
            xmlTree.Add(
                from gameServer in gameServers
                select new XElement("server",
                    new XAttribute("name", gameServer.ServerName),
                    new XElement("host", gameServer.DbHost),
                    new XElement("port", gameServer.DbPort),
                    new XElement("accountDb", gameServer.AccountDbName),
                    new XElement("gameDb", gameServer.GameDbName),
                    new XElement("user", gameServer.DbUser),
                    new XElement("password", gameServer.DbPassword)
                )
            );
            await Task.Run(async () =>
            {
                try
                {
                    using (var fileStream = File.Open(configFilePath,FileMode.Truncate))
                    {
                        xmlTree.Save(fileStream);
                        await fileStream.FlushAsync();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"保存配置文件{configFilePath}出错,{e.Message}");
                }
            });
        }
    }
}