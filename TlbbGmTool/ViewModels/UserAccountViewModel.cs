using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Models;

namespace liuguang.TlbbGmTool.ViewModels;

public class UserAccountViewModel : NotifyBase
{
    #region Fields
    private UserAccount _userAccount;
    #endregion
    #region Properties
    public int Id
    {
        get => _userAccount.Id;
        set => SetProperty(ref _userAccount.Id, value);
    }
    public string Name
    {
        get => _userAccount.Name;
        set => SetProperty(ref _userAccount.Name, value);
    }
    public string Password
    {
        get => _userAccount.Password;
        set => SetProperty(ref _userAccount.Password, value);
    }
    public string? Question
    {
        get => _userAccount.Question;
        set => SetProperty(ref _userAccount.Question, value);
    }
    public string? Answer
    {
        get => _userAccount.Answer;
        set => SetProperty(ref _userAccount.Answer, value);
    }
    public string? Email
    {
        get => _userAccount.Email;
        set => SetProperty(ref _userAccount.Email, value);
    }
    public string? IdCard
    {
        get => _userAccount.IdCard;
        set
        {
            if (SetProperty(ref _userAccount.IdCard, value))
            {
                RaisePropertyChanged(nameof(IsLock));
                RaisePropertyChanged(nameof(IsLockText));
            }
        }
    }
    public int Point
    {
        get => _userAccount.Point;
        set => SetProperty(ref _userAccount.Point, value);
    }

    public bool IsLock
    {
        get => _userAccount.IdCard == "1";
        set => IdCard = (value ? "1" : null);
    }

    public string IsLockText => (_userAccount.IdCard == "1")?"已锁定":"正常";
    #endregion

    public UserAccountViewModel(UserAccount userAccount) { _userAccount = userAccount; }

    public void CopyFrom(UserAccountViewModel src)
    {
        Id = src.Id;
        Name = src.Name;
        Password= src.Password;
        Question= src.Question;
        Answer= src.Answer;
        Email = src.Email;
        IdCard = src.IdCard;
        Point = src.Point;
    }
}
