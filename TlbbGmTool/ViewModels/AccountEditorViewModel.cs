using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Views.Account;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace liuguang.TlbbGmTool.ViewModels;
public class AccountEditorViewModel : ViewModelBase
{
    #region Fields
    private bool _isSaving = false;
    private UserAccountViewModel? _inputUserAccount;
    private readonly UserAccountViewModel _userAccount = new(new());
    /// <summary>
    /// 数据库连接
    /// </summary>
    public DbConnection? Connection;
    #endregion
    #region Properties
    public UserAccountViewModel UserAccount
    {
        get => _userAccount;
        set
        {
            _inputUserAccount = value;
            _userAccount.CopyFrom(value);
            RaisePropertyChanged(nameof(WindowTitle));
        }
    }
    public ObservableCollection<UserAccountViewModel>? AccountList { get; set; }

    public string WindowTitle => (_inputUserAccount is null) ? "添加新账号" : "修改账户信息";

    public List<ComboBoxNode<bool>> StatusSelectionList { get; } = new() {
        new("正常",false),
        new("已锁定",true),
    };

    public bool IsSaving
    {
        get => _isSaving;
        set
        {
            if (SetProperty(ref _isSaving, value))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }

    private bool InfoIsEmpty => string.IsNullOrEmpty(_userAccount.Name) || string.IsNullOrEmpty(_userAccount.Password);

    public Command ShowHashToolCommand { get; }
    public Command SaveCommand { get; }
    #endregion

    public AccountEditorViewModel()
    {
        SaveCommand = new(SaveAccount, () => !(_isSaving || InfoIsEmpty));
        ShowHashToolCommand = new(ShowHashToolDialog);
        _userAccount.PropertyChanged += UserAccount_PropertyChanged;
    }

    private void UserAccount_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_userAccount.Name) || e.PropertyName == nameof(_userAccount.Password))
        {
            SaveCommand.RaiseCanExecuteChanged();
        }
    }

    private async void SaveAccount()
    {
        if (Connection is null)
        {
            return;
        }
        IsSaving = true;
        try
        {

            if (_inputUserAccount != null)
            {
                await Task.Run(async () =>
                {
                    await UpdateAccountAsync(Connection, _userAccount);
                });
                _inputUserAccount.CopyFrom(_userAccount);
            }
            else if (AccountList != null)
            {
                await Task.Run(async () =>
                {
                    await InsertAccountAsync(Connection, _userAccount);
                });
                AccountList.Add(_userAccount);
            }
            ShowMessage("保存成功", "保存账号信息成功");
            OwnedWindow?.Close();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("保存失败", ex);
        }
        finally
        {
            IsSaving = false;
        }
    }

    private async Task UpdateAccountAsync(DbConnection connection, UserAccountViewModel userAccount)
    {
        const string sql =
            "UPDATE account SET name=@name,password=@password" +
            ",question=@question,answer=@answer,email=@email,id_card=@id_card,point=@point WHERE id=@id";
        var mySqlCommand = new MySqlCommand(sql, connection.Conn);
        //字符串参数
        var paramDictionary = new Dictionary<string, string?>
        {
            ["name"] = userAccount.Name,
            ["password"] = userAccount.Password,
            ["question"] = userAccount.Question,
            ["answer"] = userAccount.Answer,
            ["email"] = userAccount.Email,
            ["id_card"] = userAccount.IdCard,
        };
        foreach (var keypair in paramDictionary)
        {
            mySqlCommand.Parameters.Add(new MySqlParameter("@" + keypair.Key, MySqlDbType.String)
            {
                Value = keypair.Value
            });
        }
        //int类型参数
        mySqlCommand.Parameters.Add(new MySqlParameter("@point", MySqlDbType.Int32)
        {
            Value = userAccount.Point
        });
        mySqlCommand.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32)
        {
            Value = userAccount.Id
        });
        // 切换数据库
        await connection.SwitchAccountDbAsync();
        //
        await mySqlCommand.ExecuteNonQueryAsync();
    }

    private async Task InsertAccountAsync(DbConnection connection, UserAccountViewModel userAccount)
    {
        //字符串参数
        var paramDictionary = new Dictionary<string, string?>
        {
            ["name"] = userAccount.Name,
            ["password"] = userAccount.Password,
            ["question"] = userAccount.Question,
            ["answer"] = userAccount.Answer,
            ["email"] = userAccount.Email,
            ["id_card"] = userAccount.IdCard,
        };
        var fieldNames = paramDictionary.Keys.ToList();
        fieldNames.Add("point");
        var sql = "INSERT INTO account";
        sql += "(" + string.Join(", ", fieldNames) + ") VALUES";
        var fieldValueTemplates = (from fieldName in fieldNames
                                   select "@" + fieldName);
        sql += " (" + string.Join(", ", fieldValueTemplates) + ")";
        var mySqlCommand = new MySqlCommand(sql, connection.Conn);
        foreach (var keypair in paramDictionary)
        {
            mySqlCommand.Parameters.Add(new MySqlParameter("@" + keypair.Key, MySqlDbType.String)
            {
                Value = keypair.Value
            });
        }
        //int类型参数
        mySqlCommand.Parameters.Add(new MySqlParameter("@point", MySqlDbType.Int32)
        {
            Value = userAccount.Point
        });
        // 切换数据库
        await connection.SwitchAccountDbAsync();
        //
        await mySqlCommand.ExecuteNonQueryAsync();
        userAccount.Id = (int)mySqlCommand.LastInsertedId;
    }

    private void ShowHashToolDialog()
    {
        ShowDialog(new HashToolWindow());
    }
}
