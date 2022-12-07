using liuguang.TlbbGmTool.Common;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace liuguang.TlbbGmTool.ViewModels;

public class XinFaEditorViewModel : ViewModelBase
{
    #region Fields
    private bool _isSaving = false;
    private XinFaLogViewModel? _inputXinFaLog;
    private XinFaLogViewModel _xinFaLog = new(new());
    /// <summary>
    /// 数据库连接
    /// </summary>
    public DbConnection? Connection;
    #endregion
    #region Properties
    public XinFaLogViewModel XinFaLog
    {
        get => _xinFaLog;
        set
        {
            _inputXinFaLog = value;
            _xinFaLog.CopyFrom(value);
        }
    }
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
    public Command SaveCommand { get; }
    #endregion

    public XinFaEditorViewModel()
    {
        SaveCommand = new(SaveXinFa);
    }
    private async void SaveXinFa()
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
                await DoSaveXinFaAsync(Connection, _xinFaLog);
            });
            _inputXinFaLog?.CopyFrom(_xinFaLog);
            ShowMessage("保存成功", "保存心法等级成功");
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

    private async Task DoSaveXinFaAsync(DbConnection connection, XinFaLogViewModel xinFaLog)
    {
        const string sql = "UPDATE t_xinfa SET xinfalvl=@level WHERE aid=@aid";
        var mySqlCommand = new MySqlCommand(sql, connection.Conn);
        mySqlCommand.Parameters.Add(new MySqlParameter("@level", MySqlDbType.Int32)
        {
            Value = xinFaLog.XinFaLevel
        });
        mySqlCommand.Parameters.Add(new MySqlParameter("@aid", MySqlDbType.Int32)
        {
            Value = xinFaLog.Id
        });
        // 切换数据库
        await connection.SwitchGameDbAsync();
        //
        await mySqlCommand.ExecuteNonQueryAsync();
    }
}
