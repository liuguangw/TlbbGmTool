using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Documents;
using MySql.Data.MySqlClient;
using TlbbGmTool.Core;
using TlbbGmTool.Models;

namespace TlbbGmTool.ViewModels
{
    public class EditAccountViewModel : UserAccount
    {
        #region Fields

        private AccountListViewModel _accountListViewModel;
        private UserAccount _userAccount;

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

        #endregion

        public EditAccountViewModel()
        {
            SaveAccountCommand = new AppCommand(SaveToDatabase);
        }

        public void InitData(AccountListViewModel accountListViewModel, UserAccount userAccount)
        {
            _accountListViewModel = accountListViewModel;
            _userAccount = userAccount;
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
                var mainWindowViewModel = _accountListViewModel.MainWindowViewModel;
                mainWindowViewModel.showErrorMessage("保存失败", e.Message);
                return;
            }

            _userAccount.Name = Name;
            _userAccount.Password = Password;
            _userAccount.Question = Question;
            _userAccount.Answer = Answer;
            _userAccount.Email = Email;
            _userAccount.IdCard = IdCard;
            _userAccount.Point = Point;
        }

        private async Task DoSaveToDatabase()
        {
            RaisePropertyChanged(nameof(SelectedStatus));
            var mainWindowViewModel = _accountListViewModel.MainWindowViewModel;
            var mySqlConnection = mainWindowViewModel.MySqlConnection;
            const string sql = "UPDATE account SET name=@name,password=@password,question=@question,answer=@answer" +
                               ",email=@email,id_card=@idCard,point=@point WHERE id=@id";
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            //name
            var param = new MySqlParameter("@name", MySqlDbType.String)
            {
                Value = Name
            };
            mySqlCommand.Parameters.Add(param);
            //password
            param = new MySqlParameter("@password", MySqlDbType.String)
            {
                Value = Password
            };
            mySqlCommand.Parameters.Add(param);
            //question
            param = new MySqlParameter("@question", MySqlDbType.String)
            {
                Value = Question
            };
            mySqlCommand.Parameters.Add(param);
            //answer
            param = new MySqlParameter("@answer", MySqlDbType.String)
            {
                Value = Answer
            };
            mySqlCommand.Parameters.Add(param);
            //email
            param = new MySqlParameter("@email", MySqlDbType.String)
            {
                Value = Email
            };
            mySqlCommand.Parameters.Add(param);
            //id_card
            param = new MySqlParameter("@idCard", MySqlDbType.String)
            {
                Value = IdCard
            };
            mySqlCommand.Parameters.Add(param);
            //point
            param = new MySqlParameter("@point", MySqlDbType.Int32)
            {
                Value = Point
            };
            mySqlCommand.Parameters.Add(param);
            //id
            param = new MySqlParameter("@id", MySqlDbType.String)
            {
                Value = Id
            };
            mySqlCommand.Parameters.Add(param);
            await Task.Run(async () =>
            {
                var accountDbName = mainWindowViewModel.SelectedServer.AccountDbName;
                if (mySqlConnection.Database != accountDbName)
                {
                    // 切换数据库
                    await mySqlConnection.ChangeDataBaseAsync(accountDbName);
                }

                await mySqlCommand.ExecuteNonQueryAsync();
            });
        }
    }
}