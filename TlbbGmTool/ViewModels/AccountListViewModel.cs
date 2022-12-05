using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Models;
using liuguang.TlbbGmTool.Views.Account;
using MySql.Data.MySqlClient;
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

    public Command SearchCommand { get; }

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
    public Command EditAccountCommand { get; }
    #endregion

    public AccountListViewModel()
    {
        SearchCommand = new(SearchAccount, () => !_isSearching);
        EditAccountCommand = new(ShowAccountEditorDialog);
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

        using var rd = await mySqlCommand.ExecuteReaderAsync() as MySqlDataReader;
        if (rd != null)
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
        var accountInfo = parameter as UserAccountViewModel;
        if (accountInfo is null)
        {
            return;
        }
        ShowDialog(new AccountEditorWindow(), (AccountEditorViewModel vm) =>
        {
            vm.InputUserAccount = accountInfo;
            vm.Connection = Connection;
        });
    }
}
