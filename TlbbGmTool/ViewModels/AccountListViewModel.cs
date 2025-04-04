using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Models;
using liuguang.TlbbGmTool.ViewModels.Data;
using liuguang.TlbbGmTool.Views.Account;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace liuguang.TlbbGmTool.ViewModels;
public class AccountListViewModel : ViewModelBase
{
    #region Fields
    private bool _isSearching = false;
    /// <summary>
    /// 数据库连接
    /// </summary>
    public DbConnection? Connection;
    #endregion

    #region Properties
    public ObservableCollection<UserAccountViewModel> AccountList { get; } = new();

    public string SearchText { get; set; } = string.Empty;

    public bool IsSearching
    {
        set
        {
            if (SetProperty(ref _isSearching, value))
            {
                SearchCommand.RaiseCanExecuteChanged();
                AddAccountCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public Command SearchCommand { get; }
    public Command AddAccountCommand { get; }
    public Command DeleteAccountCommand { get; }
    public Command EditAccountCommand { get; }
    #endregion

    public AccountListViewModel()
    {
        SearchCommand = new(SearchAccount, () => !_isSearching);
        AddAccountCommand = new(ShowAddAccountEditorDialog, parameter => !_isSearching);
        EditAccountCommand = new(ShowAccountEditorDialog);
        DeleteAccountCommand= new(ProcessDeleteAccount);
    }

    private async void SearchAccount()
    {
        if (Connection is null)
        {
            return;
        }
        AccountList.Clear();
        IsSearching = true;
        try
        {

            var itemList = await Task.Run(async () =>
            {
                return await DoSearchAccountAsync(Connection, SearchText);
            });
            foreach (var item in itemList)
            {
                AccountList.Add(item);
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

    private async Task<List<UserAccountViewModel>> DoSearchAccountAsync(DbConnection dbConnection, string searchText)
    {
        var itemList = new List<UserAccountViewModel>();
        var sql = "SELECT * FROM account";
        if (!string.IsNullOrEmpty(searchText))
        {
            sql += " WHERE name like @searchText";
        }

        sql += " ORDER BY id ASC LIMIT 50";
        var mySqlCommand = new MySqlCommand(sql, dbConnection.Conn);
        if (!string.IsNullOrEmpty(searchText))
        {
            var searchParam = new MySqlParameter("@searchText", MySqlDbType.String)
            {
                Value = $"%{searchText}%"
            };
            mySqlCommand.Parameters.Add(searchParam);
        }
        // 切换数据库
        await dbConnection.SwitchAccountDbAsync();
        var getOptionString = (MySqlDataReader reader, string fieldName) =>
        {
            var ordinal = reader.GetOrdinal(fieldName);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        };

        using var reader = await mySqlCommand.ExecuteReaderAsync();
        if (reader is MySqlDataReader rd)
        {
            while (await rd.ReadAsync())
            {
                var userAccount = new UserAccount
                {
                    Id = rd.GetInt32("id"),
                    Name = rd.GetString("name"),
                    Password = rd.GetString("password"),
                    Question = getOptionString(rd, "question"),
                    Answer = getOptionString(rd, "answer"),
                    Email = getOptionString(rd, "email"),
                    IdCard = getOptionString(rd, "id_card"),
                    Point = rd.GetInt32("point")
                };
                //add to list
                itemList.Add(new(userAccount));
            }
        }
        return itemList;
    }

    private void ShowAccountEditorDialog(object? parameter)
    {
        if (parameter is UserAccountViewModel accountInfo)
        {
            ShowDialog(new AccountEditorWindow(), (AccountEditorViewModel vm) =>
            {
                vm.UserAccount = accountInfo;
                vm.Connection = Connection;
            });
        }
    }
    private void ShowAddAccountEditorDialog(object? parameter)
    {
        ShowDialog(new AccountEditorWindow(), (AccountEditorViewModel vm) =>
        {
            vm.AccountList = AccountList;
            vm.Connection = Connection;
        });
    }
    private async void ProcessDeleteAccount(object? parameter)
    {
        if (Connection is null)
        {
            return;
        }
        if (parameter is not UserAccountViewModel accountInfo)
        {
            return;
        }
        if (!Confirm("删除提示", $"你确定要删除账号{accountInfo.Name}吗?"))
        {
            return;
        }
        try
        {
            await Task.Run(async () =>
            {
                await DeleteAccountAsync(Connection,accountInfo.Name);
            });
            AccountList.Remove(accountInfo);
            ShowMessage("删除成功", $"删除账号{accountInfo.Name}成功");
        }
        catch (Exception ex)
        {
            ShowErrorMessage("删除失败", ex, true);
        }
    }
    private async Task DeleteAccountAsync(DbConnection connection,string name)
    {

        const string sql = "DELETE FROM account WHERE name=@name";
        var mySqlCommand = new MySqlCommand(sql, connection.Conn);
        mySqlCommand.Parameters.Add(new MySqlParameter("@name", MySqlDbType.String)
        {
            Value = name
        });
        // 切换数据库
        await connection.SwitchAccountDbAsync();
        //
        await mySqlCommand.ExecuteNonQueryAsync();
    }
}
