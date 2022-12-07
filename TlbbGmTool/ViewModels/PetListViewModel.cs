using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Services;
using MySql.Data.MySqlClient;

namespace liuguang.TlbbGmTool.ViewModels;

public class PetListViewModel : ViewModelBase
{
    #region Fields
    public int CharGuid;
    /// <summary>
    /// 数据库连接
    /// </summary>
    public DbConnection? Connection;

    #endregion

    #region Properties

    public ObservableCollection<PetLogViewModel> PetList { get; } = new();

    public Command EditPetCommand { get; }
    public Command EditPetSkillCommand { get; }
    public Command DeletePetCommand { get; }

    #endregion

    public PetListViewModel()
    {
        EditPetCommand = new(ShowPetEditor);
        EditPetSkillCommand = new(ShowPetEditor);
        DeletePetCommand = new(ShowPetEditor);
    }

    public async Task LoadPetListAsync()
    {
        if (Connection is null)
        {
            return;
        }
        try
        {
            var petList = await Task.Run(async () =>
            {
                return await DoLoadPetListAsync(Connection, CharGuid);
            });
            PetList.Clear();
            foreach (var petLog in petList)
            {
                PetList.Add(petLog);
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage("加载出错", ex);
        }
    }

    private async Task<List<PetLogViewModel>> DoLoadPetListAsync(DbConnection connection, int charGuid)
    {
        var xinFaList = new List<PetLogViewModel>();
        const string sql = "SELECT * FROM t_pet WHERE charguid=@charguid ORDER BY aid ASC";
        var mySqlCommand = new MySqlCommand(sql, connection.Conn);
        mySqlCommand.Parameters.Add(new MySqlParameter("@charguid", MySqlDbType.Int32)
        {
            Value = charGuid
        });
        // 切换数据库
        await connection.SwitchGameDbAsync();
        using (var rd = await mySqlCommand.ExecuteReaderAsync() as MySqlDataReader)
        {
            if (rd != null)
            {
                while (await rd.ReadAsync())
                {
                    xinFaList.Add(new(new()
                    {
                        Id = rd.GetInt32("aid"),
                        CharGuid = rd.GetInt32("charguid"),
                        PetName = DbStringService.ToCommonString(rd.GetString("petname")),
                        Level = rd.GetInt32("level"),
                        NeedLevel = rd.GetInt32("needlevel"),
                        AiType = rd.GetInt32("aitype"),
                        PetType = rd.GetInt32("pettype"),
                        Life = rd.GetInt32("life"),
                        Enjoy = rd.GetInt32("enjoy"),
                        Savvy = rd.GetInt32("savvy"),
                        Gengu = rd.GetInt32("gengu"),
                        GrowRate = rd.GetInt32("growrate"),
                        Repoint = rd.GetInt32("repoint"),
                        Exp = rd.GetInt32("exp"),
                        Str = rd.GetInt32("str"),
                        Spr = rd.GetInt32("spr"),
                        Con = rd.GetInt32("con"),
                        Ipr = rd.GetInt32("ipr"),
                        Dex = rd.GetInt32("dex"),
                        StrPer = rd.GetInt32("strper"),
                        SprPer = rd.GetInt32("sprper"),
                        ConPer = rd.GetInt32("conper"),
                        IprPer = rd.GetInt32("iprper"),
                        DexPer = rd.GetInt32("dexper"),
                    }));
                }
            }
        }
        return xinFaList;
    }

    private void ShowPetEditor(object? parameter)
    {
        ShowErrorMessage("todo", "todo");
    }
}
