using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Models;
using liuguang.TlbbGmTool.Services;
using liuguang.TlbbGmTool.ViewModels.Data;
using liuguang.TlbbGmTool.Views.Role;

namespace liuguang.TlbbGmTool.ViewModels;
public class RoleListViewModel : ViewModelBase
{
    #region Fields
    private bool _isSearching = false;
    /// <summary>
    /// 数据库连接
    /// </summary>
    public DbConnection? Connection;
    #endregion

    #region Properties
    public ObservableCollection<RoleViewModel> RoleList { get; } = new();
    public string RoleSearchText { get; set; } = string.Empty;
    public string AccountSearchText { get; set; } = string.Empty;
    public Command SearchCommand { get; }

    public Command EditRoleCommand { get; }

    public Command BanRoleCommand { get; }

    public Command UnBanRoleCommand { get; }

    public bool IsSearching
    {
        set
        {
            if (SetProperty(ref _isSearching, value))
            {
                SearchCommand.RaiseCanExecuteChanged();
            }
        }
    }
    #endregion

    public RoleListViewModel()
    {
        SearchCommand = new(SearchRole, () => !_isSearching);
        EditRoleCommand = new(ShowRoleWindow);
        BanRoleCommand = new(parameter => UpdateRoleBanStatus(parameter, true));
        UnBanRoleCommand = new(parameter => UpdateRoleBanStatus(parameter, false));
    }

    private async void SearchRole()
    {
        if (Connection is null)
        {
            return;
        }
        RoleList.Clear();
        IsSearching = true;
        try
        {

            var itemList = await Task.Run(async () =>
            {
                return await DoSearchRoleAsync(Connection, RoleSearchText, AccountSearchText);
            });
            foreach (var item in itemList)
            {
                RoleList.Add(item);
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage("搜索出错", ex);
        }
        finally
        {
            IsSearching = false;
        }
    }

    private async Task<List<RoleViewModel>> DoSearchRoleAsync(DbConnection dbConnection, string roleSearchText, string accountSearchText)
    {
        var roleList = new List<RoleViewModel>();
        //构造SQL语句
        var sql = "SELECT * FROM t_char";
        var searchDictionary = new Dictionary<string, string>();
        var noSearchText = (string.IsNullOrEmpty(roleSearchText) && string.IsNullOrEmpty(accountSearchText));
        if (!noSearchText)
        {
            var conditionStr = string.Empty;
            if (!string.IsNullOrEmpty(roleSearchText))
            {
                searchDictionary["charname"] = DbStringService.ToDbString(roleSearchText);
                conditionStr = "charname LIKE @charname";
            }

            if (!string.IsNullOrEmpty(accountSearchText))
            {
                searchDictionary["accname"] = accountSearchText;
                if (!string.IsNullOrEmpty(conditionStr))
                {
                    conditionStr += " AND ";
                }

                conditionStr += "accname LIKE @accname";
            }

            sql += " WHERE " + conditionStr;
        }

        sql += " ORDER BY aid ASC LIMIT 50";
        var mySqlCommand = new MySqlCommand(sql, dbConnection.Conn);
        foreach (var keyPair in searchDictionary)
        {
            mySqlCommand.Parameters.Add(new MySqlParameter($"@{keyPair.Key}", MySqlDbType.String)
            {
                Value = $"%{keyPair.Value}%"
            });
        }
        //切换数据库
        await dbConnection.SwitchGameDbAsync();

        using var reader = await mySqlCommand.ExecuteReaderAsync();
        if (reader is MySqlDataReader rd)
        {
            while (await rd.ReadAsync())
            {
                var roleInfo = new Role()
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
                };
                //add to list
                roleList.Add(new(roleInfo));
            }

        }
        return roleList;
    }

    private void ShowRoleWindow(object? parameter)
    {
        if (parameter is RoleViewModel roleInfo)
        {
            ShowDialog(new RoleWindow(), (RoleWindowViewModel vm) =>
            {
                vm.RoleInfo = roleInfo;
                vm.Connection = Connection;
            });
        }
    }

    private async void UpdateRoleBanStatus(object? parameter, bool isBanRole)
    {
        if (parameter is not RoleViewModel roleInfo)
        {
            return;
        }
        if (Connection is null)
        {
            return;
        }
        var tipText = isBanRole ? "封禁" : "解封";
        if (!Confirm("操作提示", $"你确定要{tipText}角色 {roleInfo.CharName}吗?"))
        {
            return;
        }
        try
        {
            await Task.Run(async () =>
            {
                await UpdateRoleSettingsAsync(Connection, roleInfo.CharGuid, isBanRole);
            });
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"{tipText}失败", ex);
            return;
        }

        ShowMessage($"{tipText}成功",
            $"{tipText}角色 {roleInfo.CharName}成功");
    }

    private async Task UpdateRoleSettingsAsync(DbConnection dbConnection, int charguid, bool isBanRole)
    {
        var settings = isBanRole
            ? "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000"
            : "0080F5200000040000000173010000017D01000001810100000000000000000000000000000000000116000000012300000002010000000101000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001000000FF0000000000000000000000000000D233000000000000000000000000000128B3420E0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        var sql = $"UPDATE t_char SET settings=@settings WHERE charguid={charguid}";
        var mySqlCommand = new MySqlCommand(sql, dbConnection.Conn);
        mySqlCommand.Parameters.Add(new MySqlParameter("@settings", MySqlDbType.String)
        {
            Value = settings
        });
        // 切换数据库
        await dbConnection.SwitchGameDbAsync();
        await mySqlCommand.ExecuteNonQueryAsync();
    }
}
