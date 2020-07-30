using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class AccountListViewModel : BindDataBase
    {
        #region Fields

        private string _searchText = string.Empty;
        private MainWindowViewModel _mainWindowViewModel;
        private MainWindow _mainWindow;

        #endregion


        /// <summary>
        /// account list
        /// </summary>
        public ObservableCollection<UserAccount> AccountList { get; } =
            new ObservableCollection<UserAccount>();

        #region Properties

        /// <summary>
        /// 搜索文本
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        /// <summary>
        /// 搜索命令
        /// </summary>
        public AppCommand SearchCommand { get; }

        /// <summary>
        /// 编辑账号命令
        /// </summary>
        public AppCommand EditAccountCommand { get; }

        #endregion

        public AccountListViewModel()
        {
            SearchCommand = new AppCommand(SearchAccount);
            EditAccountCommand = new AppCommand(ShowEditAccountDialog);
        }

        public void InitData(MainWindowViewModel mainWindowViewModel, MainWindow mainWindow)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _mainWindow = mainWindow;
        }

        /// <summary>
        /// 执行搜索
        /// </summary>
        private async void SearchAccount()
        {
            if (_mainWindowViewModel.ConnectionStatus != DatabaseConnectionStatus.Connected)
            {
                _mainWindowViewModel.ShowErrorMessage("出错了", "数据库未连接");
                return;
            }

            AccountList.Clear();
            try
            {
                var accountList = await DoSearchAccount();
                foreach (var accountInfo in accountList)
                {
                    AccountList.Add(accountInfo);
                }
            }
            catch (Exception e)
            {
                _mainWindowViewModel.ShowErrorMessage("搜索出错", e.Message);
            }
        }

        private async Task<List<UserAccount>> DoSearchAccount()
        {
            var accountList = new List<UserAccount>();
            var mySqlConnection = _mainWindowViewModel.MySqlConnection;
            var sql = "SELECT * FROM account";
            if (_searchText != string.Empty)
            {
                sql += " WHERE name like @searchText";
            }

            sql += " ORDER BY id ASC LIMIT 50";
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            if (_searchText != string.Empty)
            {
                var searchParam = new MySqlParameter("@searchText", MySqlDbType.String)
                {
                    Value = $"%{_searchText}%"
                };
                mySqlCommand.Parameters.Add(searchParam);
            }

            await Task.Run(async () =>
            {
                var accountDbName = _mainWindowViewModel.SelectedServer.AccountDbName;
                if (mySqlConnection.Database != accountDbName)
                {
                    // 切换数据库
                    await mySqlConnection.ChangeDataBaseAsync(accountDbName);
                }

                using (var rd = await mySqlCommand.ExecuteReaderAsync() as MySqlDataReader)
                {
                    while (await rd.ReadAsync())
                    {
                        var userAccount = new UserAccount
                        {
                            Id = rd.GetInt32("id"),
                            Name = rd.GetString("name"),
                            Password = rd.GetString("password"),
                            //Question = rd.GetString("question"),
                            //Answer = rd.GetString("answer"),
                            //Email = rd.GetString("email"),
                            Point = rd.GetInt32("point")
                        };
                        //可能为null的列
                        var ordinal = rd.GetOrdinal("question");
                        userAccount.Question = rd.IsDBNull(ordinal) ? null : rd.GetString(ordinal);
                        ordinal = rd.GetOrdinal("answer");
                        userAccount.Answer = rd.IsDBNull(ordinal) ? null : rd.GetString(ordinal);
                        ordinal = rd.GetOrdinal("email");
                        userAccount.Email = rd.IsDBNull(ordinal) ? null : rd.GetString(ordinal);
                        ordinal = rd.GetOrdinal("id_card");
                        userAccount.IdCard = rd.IsDBNull(ordinal) ? null : rd.GetString(ordinal);
                        //add to list
                        accountList.Add(userAccount);
                    }
                }
            });
            return accountList;
        }

        private void ShowEditAccountDialog(object parameter)
        {
            var userAccount = parameter as UserAccount;
            var editAccountWindow = new EditAccountWindow(_mainWindowViewModel, userAccount)
            {
                Owner = _mainWindow
            };
            editAccountWindow.ShowDialog();
        }
    }
}