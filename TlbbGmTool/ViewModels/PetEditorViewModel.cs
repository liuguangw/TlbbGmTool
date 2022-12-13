using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Services;
using liuguang.TlbbGmTool.ViewModels.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace liuguang.TlbbGmTool.ViewModels;
public class PetEditorViewModel : ViewModelBase
{
    #region Fields
    private bool _isSaving = false;
    private PetLogViewModel? _inputPetInfo;
    private PetLogViewModel _petInfo = new(new());
    private List<ComboBoxNode<int>> _aiTypeSelection = new() { 
        new("胆小",0),
        new("谨慎",1),
        new("忠诚",2),
        new("精明",3),
        new("勇敢",4)
    };
    /// <summary>
    /// 数据库连接
    /// </summary>
    public DbConnection? Connection;
    #endregion
    #region Properties
    public string WindowTitle => $"修改 {_petInfo.PetName} (ID: {_petInfo.Id})";
    public PetLogViewModel PetInfo
    {
        get => _petInfo;
        set
        {
            _inputPetInfo = value;
            _petInfo.CopyFrom(value);
            value.PropertyChanged += PetInfo_PropertyChanged;
            RaisePropertyChanged(nameof(WindowTitle));
        }
    }
    public List<ComboBoxNode<int>> AiTypeSelection => _aiTypeSelection;

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

    public PetEditorViewModel()
    {
        SaveCommand = new(SavePet, () => !_isSaving);
    }

    private void PetInfo_PropertyChanged(object? sender, PropertyChangedEventArgs evt)
    {
        PetLogViewModel? value;
        if (evt.PropertyName == nameof(value.PetName))
        {
            RaisePropertyChanged(nameof(WindowTitle));
        }
    }

    private async void SavePet()
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
                await DoSavePetAsync(Connection, _petInfo);
            });
            _inputPetInfo?.CopyFrom(_petInfo);
            ShowMessage("保存成功", "保存珍兽信息成功");
            OwnedWindow?.Close();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("保存珍兽信息失败", ex);
        }
        finally
        {
            IsSaving = false;
        }
    }

    private async Task DoSavePetAsync(DbConnection connection, PetLogViewModel petInfo)
    {
        var sql = "UPDATE t_pet SET";
        //int类型的字段
        var intDictionary = new Dictionary<string, int>()
        {
            ["level"] = petInfo.Level,
            ["needlevel"] = petInfo.NeedLevel,
            ["aitype"] = petInfo.AiType,
            ["pettype"] = petInfo.PetType,
            ["genera"] = petInfo.Genera,
            ["life"] = petInfo.Life,
            ["enjoy"] = petInfo.Enjoy,
            ["savvy"] = petInfo.Savvy,
            ["gengu"] = petInfo.Gengu,
            ["growrate"] = petInfo.GrowRate,
            ["repoint"] = petInfo.Repoint,
            ["exp"] = petInfo.Exp,
            ["str"] = petInfo.Str,
            ["spr"] = petInfo.Spr,
            ["con"] = petInfo.Con,
            ["ipr"] = petInfo.Ipr,
            ["dex"] = petInfo.Dex,
            ["strper"] = petInfo.StrPer,
            ["sprper"] = petInfo.SprPer,
            ["conper"] = petInfo.ConPer,
            ["iprper"] = petInfo.IprPer,
            ["dexper"] = petInfo.DexPer,
        };
        var fieldNames = intDictionary.Keys.ToList();
        fieldNames.Add("petname");
        // fieldA=@fieldA
        var updateCondition = (from fieldName in fieldNames
                               select $"{fieldName}=@{fieldName}");
        sql += " " + string.Join(", ", updateCondition) + " WHERE aid=@aid";
        var mySqlCommand = new MySqlCommand(sql, connection.Conn);
        foreach (var keyPair in intDictionary)
        {
            mySqlCommand.Parameters.Add(new MySqlParameter("@" + keyPair.Key, MySqlDbType.Int32)
            {
                Value = keyPair.Value
            });

        }
        mySqlCommand.Parameters.Add(new MySqlParameter("@petname", MySqlDbType.String)
        {
            Value = DbStringService.ToDbString(petInfo.PetName)
        });
        mySqlCommand.Parameters.Add(new MySqlParameter("@aid", MySqlDbType.Int32)
        {
            Value = petInfo.Id
        });
        // 切换数据库
        await connection.SwitchGameDbAsync();
        //exec
        await mySqlCommand.ExecuteNonQueryAsync();
    }
}
