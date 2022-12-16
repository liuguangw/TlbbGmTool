using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Services;
using liuguang.TlbbGmTool.Views.Pet;
using liuguang.TlbbGmTool.ViewModels.Data;
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
        EditPetSkillCommand = new(ShowPetSkillEditor);
        DeletePetCommand = new(AskDeletePet);
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
        using var reader = await mySqlCommand.ExecuteReaderAsync();
        if (reader is MySqlDataReader rd)
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
                    Genera = rd.GetInt32("genera"),
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
                    Skill = rd.GetString("skill"),
                }));
            }
        }
        return xinFaList;
    }

    private void ShowPetEditor(object? parameter)
    {
        if (parameter is PetLogViewModel petInfo)
        {
            ShowDialog(new PetEditorWindow(), (PetEditorViewModel vm) =>
            {
                vm.PetInfo = petInfo;
                vm.Connection = Connection;
            });
        }
    }

    private void ShowPetSkillEditor(object? parameter)
    {
        if (parameter is PetLogViewModel petInfo)
        {
            ShowDialog(new PetSkillEditorWindow(), (PetSkillEditorViewModel vm) =>
            {
                vm.PetInfo = petInfo;
                vm.Connection = Connection;
            });
        }
    }

    private async void AskDeletePet(object? parameter)
    {
        if (parameter is not PetLogViewModel petInfo)
        {
            return;
        }
        if (Connection is null)
        {
            return;
        }
        if (!Confirm("操作提示", $"你确定要删除珍兽{petInfo.PetName}(ID:{petInfo.Id})吗?"))
        {
            return;
        }
        try
        {
            await Task.Run(async () =>
            {
                await DeletePetAsync(Connection, petInfo);
            });
            PetList.Remove(petInfo);
            ShowMessage("删除成功", $"删除珍兽{petInfo.PetName}(ID:{petInfo.Id})成功");
        }
        catch (Exception ex)
        {
            ShowErrorMessage("删除失败", ex);
        }
    }

    private async Task DeletePetAsync(DbConnection connection, PetLogViewModel petInfo)
    {
        const string sql = "DELETE FROM t_pet WHERE aid=@aid";
        var mySqlCommand = new MySqlCommand(sql, connection.Conn);
        mySqlCommand.Parameters.Add(new MySqlParameter("@aid", MySqlDbType.Int32)
        {
            Value = petInfo.Id,
        });
        // 切换数据库
        await connection.SwitchGameDbAsync();
        //
        await mySqlCommand.ExecuteNonQueryAsync();
    }
}
