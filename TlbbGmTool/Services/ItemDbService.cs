using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.ViewModels.Data;

namespace liuguang.TlbbGmTool.Services;
public static class ItemDbService
{
    public static async Task<List<ItemLogViewModel>> LoadItemListAsync(DbConnection connection, int charGuid, int posBegin, int limitCount)
    {
        var itemList = new List<ItemLogViewModel>();
        const string sql = "SELECT * FROM t_iteminfo WHERE charguid=@charguid AND isvalid=1 AND pos>=@pos1 AND pos<@pos2 ORDER BY pos ASC";
        var mySqlCommand = new MySqlCommand(sql, connection.Conn);
        mySqlCommand.Parameters.Add(new MySqlParameter("@charguid", MySqlDbType.Int32)
        {
            Value = charGuid
        });
        mySqlCommand.Parameters.Add(new MySqlParameter("@pos1", MySqlDbType.Int32)
        {
            Value = posBegin
        });
        mySqlCommand.Parameters.Add(new MySqlParameter("@pos2", MySqlDbType.Int32)
        {
            Value = posBegin + limitCount
        });
        // 切换数据库
        await connection.SwitchGameDbAsync();
        using var reader = await mySqlCommand.ExecuteReaderAsync();
        if (reader is MySqlDataReader rd)
        {
            while (await rd.ReadAsync())
            {
                var pArray = new int[17];
                for (var i = 0; i < pArray.Length; i++)
                {
                    pArray[i] = rd.GetInt32("p" + (i + 1));
                }
                var pData = DataService.ConvertToPData(pArray);
                var serverType = connection.GameServerType;
                itemList.Add(new(new()
                {
                    Id = rd.GetInt32("aid"),
                    CharGuid = rd.GetInt32("charguid"),
                    Guid = rd.GetInt32("guid"),
                    World = rd.GetInt32("world"),
                    Server = rd.GetInt32("server"),
                    ItemBaseId = rd.GetInt32("itemtype"),
                    Pos = rd.GetInt32("pos"),
                    PData = pData,
                    Creator = DbStringService.ToCommonString(rd.GetString("creator")),
                    IsValid = (rd.GetInt32("isvalid") == 1),
                    DbVersion = rd.GetInt32("dbversion"),
                    FixAttr = rd.GetString("fixattr"),
                    TVar = rd.GetString("var"),
                    VisualId = rd.GetInt32("visualid"),
                    MaxgemId = rd.GetInt32("maxgemid")
                }, serverType));
            }
        }
        return itemList;
    }
    /// <summary>
    /// 获取下一个有效的pos
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="charGuid"></param>
    /// <param name="posBegin"></param>
    /// <param name="limitCount"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static async Task<int> GetNextPos(DbConnection connection, int charGuid, int posBegin, int limitCount)
    {
        const string sql = "SELECT pos FROM t_iteminfo WHERE charguid=@charguid AND isvalid=1 AND pos>=@pos1 AND pos<@pos2 ORDER BY pos ASC";
        var mySqlCommand = new MySqlCommand(sql, connection.Conn);
        mySqlCommand.Parameters.Add(new MySqlParameter("@charguid", MySqlDbType.Int32)
        {
            Value = charGuid
        });
        mySqlCommand.Parameters.Add(new MySqlParameter("@pos1", MySqlDbType.Int32)
        {
            Value = posBegin
        });
        var endPos = posBegin + limitCount;
        mySqlCommand.Parameters.Add(new MySqlParameter("@pos2", MySqlDbType.Int32)
        {
            Value = endPos
        });
        //初始化标记
        var findPos = false;
        var currentPos = posBegin;
        using (var reader = await mySqlCommand.ExecuteReaderAsync())
        {
            if (reader is MySqlDataReader rd)
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
        }
        if ((!findPos) && (currentPos < endPos))
        {
            findPos = true;
        }
        if (!findPos)
        {
            throw new Exception("找不到有效的pos");
        }

        //清理pos
        const string delSql = "DELETE FROM t_iteminfo WHERE charguid=@charguid AND pos=@pos";
        mySqlCommand = new MySqlCommand(delSql, connection.Conn);
        mySqlCommand.Parameters.Add(new MySqlParameter("@charguid", MySqlDbType.Int32)
        {
            Value = charGuid
        });
        mySqlCommand.Parameters.Add(new MySqlParameter("@pos", MySqlDbType.Int32)
        {
            Value = currentPos
        });
        await mySqlCommand.ExecuteNonQueryAsync();
        return currentPos;
    }

    private static async Task<int> GetNextGuid(DbConnection connection)
    {
        var nextGuid = 0;
        const string sql = "SELECT serial FROM t_itemkey WHERE smkey=7001 LIMIT 1";
        var mySqlCommand = new MySqlCommand(sql, connection.Conn);
        using (var reader = await mySqlCommand.ExecuteReaderAsync())
        {
            if (reader is MySqlDataReader rd)
            {
                if (await rd.ReadAsync())
                {
                    nextGuid = rd.GetInt32("serial");
                }
            }
        }

        //UPDATE KEY
        const string updateSql = "UPDATE t_itemkey SET serial=serial+2 WHERE smkey=7001";
        mySqlCommand = new MySqlCommand(updateSql, connection.Conn);
        await mySqlCommand.ExecuteNonQueryAsync();
        return nextGuid + 2;
    }

    private static async Task<int> GetWorldID(DbConnection connection)
    {
        var world = 0;
        const string sql = "SELECT nVal FROM t_general_set WHERE sKey='WORLD_ID' LIMIT 1";
        var mySqlCommand = new MySqlCommand(sql, connection.Conn);
        using (var reader = await mySqlCommand.ExecuteReaderAsync())
        {
            if (reader is MySqlDataReader rd)
            {
                if (await rd.ReadAsync())
                {
                    world = rd.GetInt32("nVal");
                }
            }
        }

        return world;
    }

    public static async Task InsertItemAsync(DbConnection connection, int posBegin, int limitCount, ItemLogViewModel itemLog)
    {
        var world = await GetWorldID(connection);
        var nextPos = await GetNextPos(connection, itemLog.CharGuid, posBegin, limitCount);
        var nextGuid = await GetNextGuid(connection);
        var intDictionary = new Dictionary<string, int>()
        {
            ["charguid"] = itemLog.CharGuid,
            ["guid"] = nextGuid,
            ["world"] = world,
            ["server"] = itemLog.Server,
            ["itemtype"] = itemLog.ItemBaseId,
            ["pos"] = nextPos,
        };
        var pArray = DataService.ConvertToPArray(itemLog.PData);
        for (var i = 0; i < pArray.Length; i++)
        {
            intDictionary.Add($"p{i + 1}", pArray[i]);
        }
        //P18 - P21
        if (connection.GameServerType == ServerType.HuaiJiu)
        {
            for (var i = 17; i <21; i++)
            {
                intDictionary.Add($"p{i + 1}", 0);
            }
        }
        var fieldNames = intDictionary.Keys.ToList();
        fieldNames.AddRange([
                "creator",
                "fixattr",
                "var"
            ]);
        var sql = "INSERT INTO t_iteminfo";
        sql += "(" + string.Join(", ", fieldNames) + ") VALUES";
        var fieldValueTemplates = (from fieldName in fieldNames
                                   select "@" + fieldName);
        sql += " (" + string.Join(", ", fieldValueTemplates) + ")";
        var mySqlCommand = new MySqlCommand(sql, connection.Conn);
        //int
        foreach (var keyPair in intDictionary)
        {
            mySqlCommand.Parameters.Add(new MySqlParameter("@" + keyPair.Key, MySqlDbType.Int32)
            {
                Value = keyPair.Value
            });
        }
        mySqlCommand.Parameters.Add(new MySqlParameter("@creator", MySqlDbType.String)
        {
            Value = DbStringService.ToDbString(itemLog.Creator)
        });
        mySqlCommand.Parameters.Add(new MySqlParameter("@fixattr", MySqlDbType.String)
        {
            Value = string.Empty
        });
        mySqlCommand.Parameters.Add(new MySqlParameter("@var", MySqlDbType.String)
        {
            Value = string.Empty
        });
        await mySqlCommand.ExecuteNonQueryAsync();
        itemLog.Id = (int)mySqlCommand.LastInsertedId;
        itemLog.Pos = nextPos;
        itemLog.Guid = nextGuid;
    }

    public static async Task UpdateItemAsync(DbConnection dbConnection, int logId, int itemBaseId, byte[] pData, string? creator = null)
    {
        var sql = "UPDATE t_iteminfo SET itemtype=@itemtype";
        var intDictionary = new Dictionary<string, int>()
        {
            ["@itemtype"] = itemBaseId,
            ["@aid"] = logId,
        };
        var pArray = DataService.ConvertToPArray(pData);
        for (var i = 0; i < pArray.Length; i++)
        {
            intDictionary[$"@p{i + 1}"] = pArray[i];
            sql += $",p{i + 1}=@p{i + 1}";
        }
        sql += ",creator=@creator WHERE aid=@aid";
        var mySqlCommand = new MySqlCommand(sql, dbConnection.Conn);
        foreach (var keyPair in intDictionary)
        {
            mySqlCommand.Parameters.Add(new MySqlParameter(keyPair.Key, MySqlDbType.Int32)
            {
                Value = keyPair.Value
            });
        }
        var creatorText = (creator is null) ? string.Empty : DbStringService.ToDbString(creator);
        mySqlCommand.Parameters.Add(new MySqlParameter("@creator", MySqlDbType.String)
        {
            Value = creatorText
        });
        await mySqlCommand.ExecuteNonQueryAsync();
    }
    public static async Task DeleteItemAsync(DbConnection dbConnection, int logId)
    {
        const string sql = "DELETE FROM t_iteminfo WHERE aid=@aid";
        var mySqlCommand = new MySqlCommand(sql, dbConnection.Conn);
        mySqlCommand.Parameters.Add(new MySqlParameter("@aid", MySqlDbType.Int32)
        {
            Value = logId
        });
        await mySqlCommand.ExecuteNonQueryAsync();
    }
}
