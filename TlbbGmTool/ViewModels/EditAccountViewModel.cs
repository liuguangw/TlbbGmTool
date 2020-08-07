using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class EditAccountViewModel : UserAccount
    {
        #region Fields

        private MainWindowViewModel _mainWindowViewModel;
        private UserAccount _userAccount;
        private EditAccountWindow _editAccountWindow;

        private ComboBoxNode<string> _selectedStatus = new ComboBoxNode<string>();

        #endregion

        #region Properties

        public List<ComboBoxNode<string>> StatusSelection { get; } = new List<ComboBoxNode<string>>();

        public ComboBoxNode<string> SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (SetProperty(ref _selectedStatus, value))
                {
                    IdCard = value.Value;
                }
            }
        }

        public AppCommand SaveAccountCommand { get; }
        public AppCommand ShowHashToolCommand { get; }

        #endregion

        public EditAccountViewModel()
        {
            SaveAccountCommand = new AppCommand(SaveToDatabase);
            ShowHashToolCommand = new AppCommand(ShowHashTool);
        }

        public void InitData(MainWindowViewModel mainWindowViewModel, UserAccount userAccount,
            EditAccountWindow editAccountWindow)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _userAccount = userAccount;
            _editAccountWindow = editAccountWindow;
            //初始化属性
            Id = userAccount.Id;
            Name = userAccount.Name;
            Password = userAccount.Password;
            Question = userAccount.Question;
            Answer = userAccount.Answer;
            Email = userAccount.Email;
            IdCard = userAccount.IdCard ?? string.Empty;
            Point = userAccount.Point;
            //初始化状态下拉框
            var commonStatus = new ComboBoxNode<string>
            {
                Title = "正常",
                Value = string.Empty
            };
            var lockStatus = new ComboBoxNode<string>
            {
                Title = "已锁定",
                Value = "1"
            };
            StatusSelection.Add(commonStatus);
            StatusSelection.Add(lockStatus);
            RaisePropertyChanged(nameof(StatusSelection));
            //初始化状态值
            SelectedStatus = IsLock ? lockStatus : commonStatus;
        }

        private async void SaveToDatabase()
        {
            try
            {
                await DoSaveToDatabase();
            }
            catch (Exception e)
            {
                _mainWindowViewModel.ShowErrorMessage("保存失败", e.Message);
                return;
            }

            _userAccount.Name = Name;
            _userAccount.Password = Password;
            _userAccount.Question = Question;
            _userAccount.Answer = Answer;
            _userAccount.Email = Email;
            _userAccount.IdCard = IdCard;
            _userAccount.Point = Point;
            _mainWindowViewModel.ShowSuccessMessage("保存成功", "保存账号信息成功");
            _editAccountWindow.Close();
        }

        private async Task DoSaveToDatabase()
        {
            var mySqlConnection = _mainWindowViewModel.MySqlConnection;
            const string sql =
                "UPDATE account SET name=@name,password=@password" +
                ",question=@question,answer=@answer,email=@email,id_card=@idCard,point=@point WHERE id=@id";
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            //字符串参数
            var paramDictionary = new Dictionary<string, string>
            {
                ["name"] = Name,
                ["password"] = Password,
                ["question"] = Question,
                ["answer"] = Answer,
                ["email"] = Email,
                ["idCard"] = IdCard,
            };
            var mySqlParameters = (from paramInfo in paramDictionary
                select new MySqlParameter("@" + paramInfo.Key, MySqlDbType.String)
                {
                    Value = paramInfo.Value
                }).ToList();
            //int类型参数
            mySqlParameters.Add(new MySqlParameter("@point", MySqlDbType.Int32)
            {
                Value = Point
            });
            mySqlParameters.Add(new MySqlParameter("@id", MySqlDbType.Int32)
            {
                Value = Id
            });
            mySqlParameters.ForEach(mySqlParameter => mySqlCommand.Parameters.Add(mySqlParameter));
            await Task.Run(async () =>
            {
                var accountDbName = _mainWindowViewModel.SelectedServer.AccountDbName;
                if (mySqlConnection.Database != accountDbName)
                {
                    // 切换数据库
                    await mySqlConnection.ChangeDataBaseAsync(accountDbName);
                }

                await mySqlCommand.ExecuteNonQueryAsync();
            });
        }

        private void ShowHashTool()
        {
            var hashToolWindow = new HashToolWindow()
            {
                Owner = _editAccountWindow
            };
            hashToolWindow.ShowDialog();
        }
    }
}