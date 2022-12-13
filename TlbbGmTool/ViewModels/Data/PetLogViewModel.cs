using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Models;

namespace liuguang.TlbbGmTool.ViewModels.Data;

public class PetLogViewModel : NotifyBase
{
    #region Fields
    private PetLog _petLog;
    #endregion
    #region Properties
    public int Id
    {
        get => _petLog.Id;
        private set => SetProperty(ref _petLog.Id, value);
    }
    public int CharGuid
    {
        get => _petLog.CharGuid;
        private set => SetProperty(ref _petLog.CharGuid, value);
    }
    public string PetName
    {
        get => _petLog.PetName; set => SetProperty(ref _petLog.PetName, value);
    }
    public int Level
    {
        get => _petLog.Level;set => SetProperty(ref _petLog.Level, value);
    }
    public int NeedLevel
    {
        get => _petLog.NeedLevel; set => SetProperty(ref _petLog.NeedLevel, value);
    }
    public int AiType
    {
        get => _petLog.AiType; set => SetProperty(ref _petLog.AiType, value);
    }
    public int PetType
    {
        get => _petLog.PetType; set => SetProperty(ref _petLog.PetType, value);
    }
    public int Genera
    {
        get => _petLog.Genera; set => SetProperty(ref _petLog.Genera, value);
    }
    public int Life
    {
        get => _petLog.Life; set => SetProperty(ref _petLog.Life, value);
    }
    public int Enjoy
    {
        get => _petLog.Enjoy;set => SetProperty(ref _petLog.Enjoy, value);
    }

    public int Savvy
    {
        get => _petLog.Savvy; set => SetProperty(ref _petLog.Savvy, value);
    }
    public int Gengu
    {
        get => _petLog.Gengu;set => SetProperty(ref _petLog.Gengu, value);
    }
    public int GrowRate
    {
        get => _petLog.GrowRate; set => SetProperty(ref _petLog.GrowRate, value);
    }
    public int Repoint
    {
        get => _petLog.Repoint; set => SetProperty(ref _petLog.Repoint, value);
    }
    public int Exp
    {
        get => _petLog.Exp; set => SetProperty(ref _petLog.Exp, value);
    }

    public int Str
    {
        get => _petLog.Str; set => SetProperty(ref _petLog.Str, value);
    }
    public int Spr
    {
        get => _petLog.Spr; set => SetProperty(ref _petLog.Spr, value);
    }
    public int Con
    {
        get => _petLog.Con; set => SetProperty(ref _petLog.Con, value);
    }
    public int Ipr
    {
        get => _petLog.Ipr; set => SetProperty(ref _petLog.Ipr, value);
    }
    public int Dex
    {
        get => _petLog.Dex;set => SetProperty(ref _petLog.Dex, value);
    }

    public int StrPer
    {
        get => _petLog.StrPer; set => SetProperty(ref _petLog.StrPer, value);
    }
    public int SprPer
    {
        get => _petLog.SprPer; set => SetProperty(ref _petLog.SprPer, value);
    }
    public int ConPer
    {
        get => _petLog.ConPer; set => SetProperty(ref _petLog.ConPer, value);
    }
    public int IprPer
    {
        get => _petLog.IprPer;set => SetProperty(ref _petLog.IprPer, value);
    }
    public int DexPer
    {
        get => _petLog.DexPer; set => SetProperty(ref _petLog.DexPer, value);
    }
    public string Skill
    {
        get => _petLog.Skill;
        set
        {
            _petLog.Skill = value;
        }
    }
    #endregion

    public PetLogViewModel(PetLog petLog)
    {
        _petLog = petLog;
    }
    public void CopyFrom(PetLogViewModel src)
    {
        Id = src.Id;
        CharGuid = src.CharGuid;
        PetName = src.PetName;
        Level = src.Level;
        NeedLevel = src.NeedLevel;
        AiType = src.AiType;
        PetType = src.PetType;
        Genera = src.Genera;
        Life = src.Life;
        Enjoy = src.Enjoy;
        Savvy = src.Savvy;
        Gengu = src.Gengu;
        GrowRate = src.GrowRate;
        Repoint = src.Repoint;
        Exp = src.Exp;
        //
        Str = src.Str;
        Spr = src.Spr;
        Con = src.Con;
        Ipr = src.Ipr;
        Dex = src.Dex;
        //
        StrPer = src.StrPer;
        SprPer = src.SprPer;
        ConPer = src.ConPer;
        IprPer = src.IprPer;
        DexPer = src.DexPer;
        //
        Skill = src.Skill;
    }
}
