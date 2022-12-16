using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Services;
using liuguang.TlbbGmTool.ViewModels.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace liuguang.TlbbGmTool.ViewModels;
public class RoleEditorViewModel : ViewModelBase
{
    #region Fields
    private bool _isSaving = false;
    private RoleViewModel? _inputRoleInfo;
    private RoleViewModel _roleInfo = new(new());
    private List<ComboBoxNode<int>> _menpaiSelection = new();
    /// <summary>
    /// 数据库连接
    /// </summary>
    public DbConnection? Connection;
    #endregion
    #region Properties
    public RoleViewModel RoleInfo
    {
        get => _roleInfo;
        set
        {
            _inputRoleInfo = value;
            _roleInfo.CopyFrom(value);
        }
    }
    public List<ComboBoxNode<int>> MenpaiSelection => _menpaiSelection;

    public bool IsSaving
    {
        get => _isSaving;
        set
        {
            if (SetProperty(ref _isSaving, value))
            {
                SaveRoleCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public Command GoHomeCommand { get; }

    public Command SaveRoleCommand { get; }
    #endregion

    public RoleEditorViewModel()
    {
        foreach (var keyPair in SharedData.MenpaiMap)
        {
            _menpaiSelection.Add(new(keyPair.Value, keyPair.Key));
        }
        GoHomeCommand = new(GoHome);
        SaveRoleCommand = new(SaveRole, () => !_isSaving);
    }

    /// <summary>
    /// 从数据库中读取最新的角色信息
    /// </summary>
    /// <returns></returns>
    public async Task LoadRoleInfoAsync()
    {
        var charGuid = _roleInfo.CharGuid;
        if (Connection is null)
        {
            return;
        }
        try
        {
            var dbRoleInfo = await Task.Run(async () =>
            {
                return await LoadRoleInfoAsync(Connection, charGuid);
            });
            if (dbRoleInfo != null)
            {
                _inputRoleInfo?.CopyFrom(dbRoleInfo);
                _roleInfo.CopyFrom(dbRoleInfo);
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage("加载出错", ex);
        }
    }

    private async Task<RoleViewModel?> LoadRoleInfoAsync(DbConnection dbConnection, int charGuid)
    {
        //构造SQL语句
        const string sql = "SELECT * FROM t_char WHERE charguid=@charguid";
        var mySqlCommand = new MySqlCommand(sql, dbConnection.Conn);
        mySqlCommand.Parameters.Add(new MySqlParameter("@charguid", MySqlDbType.Int32)
        {
            Value = charGuid
        });
        RoleViewModel? dbRoleInfo = null;
        using var reader = await mySqlCommand.ExecuteReaderAsync();
        if (reader is MySqlDataReader rd)
        {
            if (await rd.ReadAsync())
            {
                dbRoleInfo = new(new()
                {
                    AccName = rd.GetString("accname"),
                    CharGuid = rd.GetInt32("charguid"),
                    CharName = DbStringService.ToCommonString(rd.GetString("charname")),
                    Title = DbStringService.ToCommonString(rd.GetString("title")),
                    Menpai = rd.GetInt32("menpai"),
                    Level = rd.GetInt32("level"),
                    Scene = rd.GetInt32("scene"),
                    XPos = rd.GetInt32("xpos"),
                    ZPos = rd.GetInt32("zpos"),
                    Hp = rd.GetInt32("hp"),
                    Mp = rd.GetInt32("mp"),
                    Str = rd.GetInt32("str"),
                    Spr = rd.GetInt32("spr"),
                    Con = rd.GetInt32("con"),
                    Ipr = rd.GetInt32("ipr"),
                    Dex = rd.GetInt32("dex"),
                    Points = rd.GetInt32("points"),
                    Enegry = rd.GetInt32("enegry"),
                    EnergyMax = rd.GetInt32("energymax"),
                    Vigor = rd.GetInt32("vigor"),
                    MaxVigor = rd.GetInt32("maxvigor"),
                    Exp = rd.GetInt32("exp"),
                    PkValue = rd.GetInt32("pkvalue"),
                    VMoney = rd.GetInt32("vmoney"),
                    BankMoney = rd.GetInt32("bankmoney"),
                    YuanBao = rd.GetInt32("yuanbao"),
                    MenpaiPoint = rd.GetInt32("menpaipoint"),
                    ZengDian = rd.GetInt32("zengdian"),
                });
            }
        }
        return dbRoleInfo;
    }

    private void GoHome()
    {
        _roleInfo.Scene = 2;
        _roleInfo.XPos = 160 * 100;
        _roleInfo.ZPos = 149 * 100;
    }
    private async void SaveRole()
    {
        if (Connection is null)
        {
            return;
        }
        IsSaving = true;
        try
        {
            await Task.Run(async () =>
            {
                await DoSaveRoleAsync(Connection, _roleInfo);
            });
            _inputRoleInfo?.CopyFrom(_roleInfo);
            ShowMessage("保存成功", "保存角色信息成功");
        }
        catch (Exception ex)
        {
            ShowErrorMessage("保存角色失败", ex);
        }
        finally
        {
            IsSaving = false;
        }
    }

    private async Task DoSaveRoleAsync(DbConnection connection, RoleViewModel roleInfo)
    {
        var sql = "UPDATE t_char SET";
        //int类型的字段
        var intDictionary = new Dictionary<string, int>()
        {
            ["menpai"] = roleInfo.Menpai,
            ["level"] = roleInfo.Level,
            ["scene"] = roleInfo.Scene,
            ["xpos"] = roleInfo.XPos,
            ["zpos"] = roleInfo.ZPos,
            ["hp"] = roleInfo.Hp,
            ["mp"] = roleInfo.Mp,
            ["str"] = roleInfo.Str,
            ["spr"] = roleInfo.Spr,
            ["con"] = roleInfo.Con,
            ["ipr"] = roleInfo.Ipr,
            ["dex"] = roleInfo.Dex,
            ["points"] = roleInfo.Points,
            ["enegry"] = roleInfo.Enegry,
            ["energymax"] = roleInfo.EnergyMax,
            ["vigor"] = roleInfo.Vigor,
            ["maxvigor"] = roleInfo.MaxVigor,
            ["exp"] = roleInfo.Exp,
            ["pkvalue"] = roleInfo.PkValue,
            ["vmoney"] = roleInfo.VMoney,
            ["bankmoney"] = roleInfo.BankMoney,
            ["yuanbao"] = roleInfo.YuanBao,
            ["menpaipoint"] = roleInfo.MenpaiPoint,
            ["zengdian"] = roleInfo.ZengDian
        };
        var fieldNames = intDictionary.Keys.ToList();
        fieldNames.AddRange(new[] { "accname", "charname", "title" });
        // fieldA=@fieldA
        var updateCondition = (from fieldName in fieldNames
                               select $"{fieldName}=@{fieldName}");
        sql += " " + string.Join(", ", updateCondition) + " WHERE charguid=@charguid";
        var mySqlCommand = new MySqlCommand(sql, connection.Conn);
        //构造参数
        intDictionary["charguid"] = roleInfo.CharGuid;
        foreach (var keyPair in intDictionary)
        {
            mySqlCommand.Parameters.Add(new MySqlParameter("@" + keyPair.Key, MySqlDbType.Int32)
            {
                Value = keyPair.Value
            });

        }
        mySqlCommand.Parameters.Add(new MySqlParameter("@accname", MySqlDbType.String)
        {
            Value = DbStringService.ToDbString(roleInfo.AccName)
        });
        mySqlCommand.Parameters.Add(new MySqlParameter("@charname", MySqlDbType.String)
        {
            Value = DbStringService.ToDbString(roleInfo.CharName)
        });
        mySqlCommand.Parameters.Add(new MySqlParameter("@title", MySqlDbType.String)
        {
            Value = DbStringService.ToDbString(roleInfo.Title)
        });
        // 切换数据库
        await connection.SwitchGameDbAsync();
        //exec
        await mySqlCommand.ExecuteNonQueryAsync();
    }
}
