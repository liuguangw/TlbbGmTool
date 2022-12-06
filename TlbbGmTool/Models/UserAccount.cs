namespace liuguang.TlbbGmTool.Models;

/// <summary>
/// 一条账号记录
/// </summary>
public class UserAccount
{
    public int Id;
    public string Name = string.Empty;
    public string Password = string.Empty;
    public string? Question;
    public string? Answer;
    public string? Email;
    public string? IdCard;
    public int Point;
}
