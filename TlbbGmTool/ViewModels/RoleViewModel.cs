using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Models;
using System.Collections.Generic;

namespace liuguang.TlbbGmTool.ViewModels;
public class RoleViewModel : NotifyBase
{
    #region Fields
    private Role _role;
    private SortedDictionary<int, string> _menpaiMap;
    #endregion
    #region Properties
    public string AccName
    {
        get => _role.AccName; set => SetProperty(ref _role.AccName, value);
    }
    public int CharGuid
    {
        get => _role.CharGuid; set => SetProperty(ref _role.CharGuid, value);
    }
    public string CharName
    {
        get => _role.CharName; set => SetProperty(ref _role.CharName, value);
    }
    public string Title
    {
        get => _role.Title; set => SetProperty(ref _role.Title, value);
    }
    public int Menpai
    {
        get => _role.Menpai; set
        {
            if (SetProperty(ref _role.Menpai, value))
            {
                RaisePropertyChanged(nameof(MenpaiText));
            }
        }
    }

    public string MenpaiText
    {
        get
        {
            if (_menpaiMap.TryGetValue(_role.Menpai, out var menPaiText))
            {
                return menPaiText;
            }
            return string.Empty;
        }
    }
    public int Level
    {
        get => _role.Level; set => SetProperty(ref _role.Level, value);
    }
    //
    public int Scene
    {
        get => _role.Scene; set => SetProperty(ref _role.Scene, value);
    }
    public int XPos
    {
        get => _role.XPos; set => SetProperty(ref _role.XPos, value);
    }
    public int ZPos
    {
        get => _role.ZPos; set => SetProperty(ref _role.ZPos, value);
    }

    //
    public int Hp
    {
        get => _role.Hp; set => SetProperty(ref _role.Hp, value);
    }
    public int Mp
    {
        get => _role.Mp; set => SetProperty(ref _role.Mp, value);
    }

    //
    public int Str
    {
        get => _role.Str; set => SetProperty(ref _role.Str, value);
    }
    public int Spr
    {
        get => _role.Spr; set => SetProperty(ref _role.Spr, value);
    }
    public int Con
    {
        get => _role.Con; set => SetProperty(ref _role.Con, value);
    }
    public int Ipr
    {
        get => _role.Ipr; set => SetProperty(ref _role.Ipr, value);
    }
    public int Dex
    {
        get => _role.Dex; set => SetProperty(ref _role.Dex, value);
    }

    public int Points
    {
        get => _role.Points; set => SetProperty(ref _role.Points, value);
    }

    //
    public int Enegry
    {
        get => _role.Enegry; set => SetProperty(ref _role.Enegry, value);
    }
    public int EnergyMax
    {
        get => _role.EnergyMax; set => SetProperty(ref _role.EnergyMax, value);
    }
    public int Vigor
    {
        get => _role.Vigor; set => SetProperty(ref _role.Vigor, value);
    }
    public int MaxVigor
    {
        get => _role.MaxVigor; set => SetProperty(ref _role.MaxVigor, value);
    }

    //
    public int Exp
    {
        get => _role.Exp; set => SetProperty(ref _role.Exp, value);
    }
    public int PkValue
    {
        get => _role.PkValue; set => SetProperty(ref _role.PkValue, value);
    }
    public int VMoney
    {
        get => _role.VMoney; set => SetProperty(ref _role.VMoney, value);
    }
    public int BankMoney
    {
        get => _role.BankMoney; set => SetProperty(ref _role.BankMoney, value);
    }
    public int YuanBao
    {
        get => _role.YuanBao; set => SetProperty(ref _role.YuanBao, value);
    }
    public int MenpaiPoint
    {
        get => _role.MenpaiPoint; set => SetProperty(ref _role.MenpaiPoint, value);
    }
    public int ZengDian
    {
        get => _role.ZengDian; set => SetProperty(ref _role.ZengDian, value);
    }
    #endregion

    public RoleViewModel(Role role, SortedDictionary<int, string> menpaiMap)
    {
        _role = role;
        _menpaiMap = menpaiMap;
    }
}
