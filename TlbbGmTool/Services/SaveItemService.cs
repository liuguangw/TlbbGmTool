using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TlbbGmTool.Models;

namespace TlbbGmTool.Services
{
    public class SaveItemService
    {
        public enum BagType
        {
            ItemBag,
            MaterialBag,
            TaskBag
        }

        public static async Task PrepareConnection(MySqlConnection mySqlConnection, string gameDbName)
        {
            if (mySqlConnection.Database != gameDbName)
            {
                // 切换数据库
                await mySqlConnection.ChangeDataBaseAsync(gameDbName);
            }
        }

        /// <summary>
        /// 获取下一个有效的pos
        /// </summary>
        /// <param name="mySqlConnection"></param>
        /// <param name="charguid"></param>
        /// <param name="bagType"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static async Task<int> GetNextPos(MySqlConnection mySqlConnection, int charguid, BagType bagType)
        {
            var (startPos, endPos) = GetBagItemIndexRange(bagType);
            var currentPos = startPos;
            var findPos = false;
            var sql = $"SELECT pos FROM t_iteminfo WHERE charguid={charguid}"
                      + " AND isvalid=1"
                      + $" AND pos>={startPos} AND pos<{endPos}"
                      + " ORDER BY pos ASC";
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            await Task.Run(async () =>
            {
                using (var rd = await mySqlCommand.ExecuteReaderAsync() as MySqlDataReader)
                {
                    while (await rd.ReadAsync())
                    {
                        var pos = rd.GetInt32("pos");
                        if (currentPos < pos)
                        {
                            findPos = true;
                            break;
                        }

                        currentPos++;
                    }
                }
            });
            if (!findPos && currentPos < endPos)
            {
                findPos = true;
            }
            else if (!findPos)
            {
                throw new Exception("找不到有效的pos");
            }

            //清理pos
            sql = $"DELETE FROM t_iteminfo WHERE charguid={charguid} AND pos={currentPos}";
            mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            await Task.Run(async () => { await mySqlCommand.ExecuteNonQueryAsync(); });
            return currentPos;
        }

        private static async Task<int> GetNextGuid(MySqlConnection mySqlConnection)
        {
            var nextGuid = 0;
            const string sql = "SELECT serial FROM t_itemkey WHERE smkey=7001 LIMIT 1";
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            await Task.Run(async () =>
            {
                using (var rd = await mySqlCommand.ExecuteReaderAsync() as MySqlDataReader)
                {
                    if (await rd.ReadAsync())
                    {
                        nextGuid = rd.GetInt32("serial");
                    }
                }

                //UPDATE KEY
                var updateSql = "UPDATE t_itemkey SET serial=serial+2 WHERE smkey=7001";
                mySqlCommand = new MySqlCommand(updateSql, mySqlConnection);
                await mySqlCommand.ExecuteNonQueryAsync();
            });
            return nextGuid + 2;
        }


        /// <summary>
        /// 插入item
        /// </summary>
        /// <param name="mySqlConnection">数据库连接</param>
        /// <param name="itemType">物品ID</param>
        /// <param name="pArray">数据数组</param>
        /// <param name="charguid"></param>
        /// <param name="itemBases"></param>
        /// <param name="itemBagType"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public static async Task<ItemInfo> InsertItemAsync(MySqlConnection mySqlConnection, int itemType, int[] pArray,
            int charguid, Dictionary<int, ItemBase> itemBases, BagType itemBagType, string creator)
        {
            var pos = await GetNextPos(mySqlConnection, charguid, itemBagType);
            var guid = await GetNextGuid(mySqlConnection);
            var intDictionary = new Dictionary<string, int>()
            {
                ["charguid"] = charguid,
                ["guid"] = guid,
                ["world"] = 101,
                ["server"] = 0,
                ["itemtype"] = itemType,
                ["pos"] = pos,
            };
            for (var i = 0; i < pArray.Length; i++)
            {
                intDictionary.Add($"p{i + 1}", pArray[i]);
            }

            var fieldNames = intDictionary.Keys.ToList();
            fieldNames.AddRange(new[]
            {
                "creator",
                "fixattr",
                "var"
            });
            var sql = "INSERT INTO t_iteminfo";
            sql += "(" + string.Join(", ", fieldNames) + ") VALUES";
            var fieldValueTemplates = (from fieldName in fieldNames
                select "@" + fieldName);
            sql += " (" + string.Join(", ", fieldValueTemplates) + ")";
            //构造参数
            var mySqlParameters = (from intParameter in intDictionary
                select new MySqlParameter("@" + intParameter.Key, MySqlDbType.Int32)
                {
                    Value = intParameter.Value
                }).ToList();
            mySqlParameters.Add(new MySqlParameter("@creator", MySqlDbType.String)
            {
                Value = DbStringService.ToDbString(creator)
            });
            mySqlParameters.Add(new MySqlParameter("@fixattr", MySqlDbType.String)
            {
                Value = string.Empty
            });
            mySqlParameters.Add(new MySqlParameter("@var", MySqlDbType.String)
            {
                Value = string.Empty
            });
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            mySqlParameters.ForEach(mySqlParameter => mySqlCommand.Parameters.Add(mySqlParameter));
            await Task.Run(async () => { await mySqlCommand.ExecuteNonQueryAsync(); });
            return new ItemInfo(itemBases)
            {
                Charguid = charguid,
                Guid = guid,
                World = intDictionary["world"],
                Server = intDictionary["server"],
                ItemType = itemType,
                Pos = intDictionary["pos"],
                PArray = pArray,
                Creator = creator
            };
        }

        /// <summary>
        /// 更新item
        /// </summary>
        /// <param name="mySqlConnection">数据库连接</param>
        /// <param name="itemType">物品ID</param>
        /// <param name="pArray">数据数组</param>
        /// <param name="charguid"></param>
        /// <param name="itemBases"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static async Task<ItemInfo> UpdateItemAsync(MySqlConnection mySqlConnection, int itemType, int[] pArray,
            int charguid, Dictionary<int, ItemBase> itemBases, int pos)
        {
            var sql = "UPDATE t_iteminfo SET itemtype=@itemtype";
            var intDictionary = new Dictionary<string, int>();
            for (var i = 0; i < pArray.Length; i++)
            {
                intDictionary[$"p{i + 1}"] = pArray[i];
            }

            foreach (var keyValuePair in intDictionary)
            {
                sql += $",{keyValuePair.Key}=@{keyValuePair.Key}";
            }

            sql += " WHERE charguid=@charguid AND pos=@pos";
            intDictionary.Add("itemtype", itemType);
            intDictionary.Add("charguid", charguid);
            intDictionary.Add("pos", pos);
            //构造参数
            var mySqlParameters = (from intParameter in intDictionary
                select new MySqlParameter("@" + intParameter.Key, MySqlDbType.Int32)
                {
                    Value = intParameter.Value
                }).ToList();
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            mySqlParameters.ForEach(mySqlParameter => mySqlCommand.Parameters.Add(mySqlParameter));
            await Task.Run(async () => { await mySqlCommand.ExecuteNonQueryAsync(); });
            return new ItemInfo(itemBases)
            {
                ItemType = itemType,
                PArray = pArray,
                Pos = pos
            };
        }

        public static async Task<ItemInfo> CopyItemAsync(MySqlConnection mySqlConnection,
            Dictionary<int, ItemBase> itemBases, ItemInfo sourceItem, BagType itemBagType)
        {
            var charguid = sourceItem.Charguid;
            var pos = await GetNextPos(mySqlConnection, charguid, itemBagType);
            var guid = await GetNextGuid(mySqlConnection);
            var intDictionary = new Dictionary<string, int>()
            {
                ["charguid"] = charguid,
                ["guid"] = guid,
                ["world"] = sourceItem.World,
                ["server"] = sourceItem.Server,
                ["itemtype"] = sourceItem.ItemType,
                ["pos"] = pos,
            };
            var pArray = sourceItem.PArray;
            for (var i = 0; i < pArray.Length; i++)
            {
                intDictionary.Add($"p{i + 1}", pArray[i]);
            }

            var fieldNames = intDictionary.Keys.ToList();
            fieldNames.AddRange(new[]
            {
                "creator",
                "fixattr",
                "var"
            });
            var sql = "INSERT INTO t_iteminfo";
            sql += "(" + string.Join(", ", fieldNames) + ") VALUES";
            var fieldValueTemplates = (from fieldName in fieldNames
                select "@" + fieldName);
            sql += " (" + string.Join(", ", fieldValueTemplates) + ")";
            //构造参数
            var mySqlParameters = (from intParameter in intDictionary
                select new MySqlParameter("@" + intParameter.Key, MySqlDbType.Int32)
                {
                    Value = intParameter.Value
                }).ToList();
            mySqlParameters.Add(new MySqlParameter("@creator", MySqlDbType.String)
            {
                Value = DbStringService.ToDbString(sourceItem.Creator)
            });
            mySqlParameters.Add(new MySqlParameter("@fixattr", MySqlDbType.String)
            {
                Value = string.Empty
            });
            mySqlParameters.Add(new MySqlParameter("@var", MySqlDbType.String)
            {
                Value = string.Empty
            });
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            mySqlParameters.ForEach(mySqlParameter => mySqlCommand.Parameters.Add(mySqlParameter));
            await Task.Run(async () => { await mySqlCommand.ExecuteNonQueryAsync(); });
            return new ItemInfo(itemBases)
            {
                Charguid = charguid,
                Guid = guid,
                World = intDictionary["world"],
                Server = intDictionary["server"],
                ItemType = sourceItem.ItemType,
                Pos = intDictionary["pos"],
                PArray = pArray,
                Creator = sourceItem.Creator
            };
        }


        public static (int, int) GetBagItemIndexRange(BagType bagType)
        {
            var startPos = 0;
            switch (bagType)
            {
                case BagType.MaterialBag:
                    startPos = 30;
                    break;
                case BagType.TaskBag:
                    startPos = 60;
                    break;
            }

            var endPos = startPos + 30;
            return (startPos, endPos);
        }
    }
}