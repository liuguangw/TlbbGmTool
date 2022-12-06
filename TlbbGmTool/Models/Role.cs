namespace liuguang.TlbbGmTool.Models;

/// <summary>
/// 角色数据记录
/// </summary>
public class Role
{
    public string AccName = string.Empty;
    public int CharGuid;
    public string CharName = string.Empty;
    public string Title = string.Empty;
    public int Menpai;
    public int Level;

    //
    public int Scene;
    public int XPos;
    public int ZPos;

    //
    public int Hp;
    public int Mp;

    //
    public int Str;
    public int Spr;
    public int Con;
    public int Ipr;
    public int Dex;

    public int Points;

    //
    public int Enegry;
    public int EnergyMax;
    public int Vigor;
    public int MaxVigor;

    //
    public int Exp;
    public int PkValue;
    public int VMoney;
    public int BankMoney;
    public int YuanBao;
    public int MenpaiPoint;
    public int ZengDian;
}
