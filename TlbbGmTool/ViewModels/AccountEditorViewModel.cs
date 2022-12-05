using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Views.Account;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace liuguang.TlbbGmTool.ViewModels;
public class AccountEditorViewModel : ViewModelBase
{
    #region Fields
    private bool _isSaving = false;
    private UserAccountViewModel? _inputUserAccount;
    /// <summary>
    /// 数据库连接
    /// </summary>
    public DbConnection? Connection;
    #endregion
    #region Properties
    public UserAccountViewModel InputUserAccount
    {
        set
        {
            if (SetProperty(ref _inputUserAccount, value))
            {
                UserAccount.CopyFrom(value);
            }
        }
    }
    public UserAccountViewModel UserAccount { get; } = new(new());

    public List<ComboBoxNode<bool>> StatusSelectionList { get; } = new() {
        new("正常",false),
        new("已锁定",true),
    };

    public bool IsSaving {
        get => _isSaving;
        set
        {
            if(SetProperty(ref _isSaving, value))
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public Command ShowHashToolCommand { get; }
    public Command SaveCommand { get; }
    #endregion

    public AccountEditorViewModel()
    {
        SaveCommand = new(SaveAccount, () => !_isSaving);
        ShowHashToolCommand = new(ShowHashToolDialog);
    }
    private async void SaveAccount()
    {
        if ((Connection is null)||(_inputUserAccount is null))
        {
            return;
        }
        IsSaving = true;
        try
        {

            await Task.Run(async () =>
            {
                await DoSaveAccountAsync(Connection);
            });
            _inputUserAccount.CopyFrom(UserAccount);
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

    private async Task DoSaveAccountAsync(DbConnection dbConnection)
    {
        const string sql =
            "UPDATE account SET name=@name,password=@password" +
            ",question=@question,answer=@answer,email=@email,id_card=@idCard,point=@point WHERE id=@id";
        var mySqlCommand = new MySqlCommand(sql, dbConnection.Conn);
        //字符串参数
        var paramDictionary = new Dictionary<string, string?>
        {
            ["name"] = UserAccount.Name,
            ["password"] = UserAccount.Password,
            ["question"] = UserAccount.Question,
            ["answer"] = UserAccount.Answer,
            ["email"] = UserAccount.Email,
            ["idCard"] = UserAccount.IdCard,
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
            Value = UserAccount.Point
        });
        mySqlCommand.Parameters.Add(new MySqlParameter("@id", MySqlDbType.Int32)
        {
            Value = UserAccount.Id
        });
        // 切换数据库
        await dbConnection.SwitchAccountDbAsync();
        //
        await mySqlCommand.ExecuteNonQueryAsync();
    }

    private void ShowHashToolDialog()
    {
        ShowDialog(new HashToolWindow());
    }
}
