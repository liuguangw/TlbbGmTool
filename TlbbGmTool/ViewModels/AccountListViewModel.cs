using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TlbbGmTool.Core;
using TlbbGmTool.Models;

namespace TlbbGmTool.ViewModels
{
    public class AccountListViewModel : BindDataBase
    {
        #region Fields

        private string _searchText = string.Empty;

        #endregion


        /// <summary>
        /// account list
        /// </summary>
        public ObservableCollection<UserAccount> AccountList { get; } =
            new ObservableCollection<UserAccount>();

        /// <summary>
        /// Main window ViewModel
        /// </summary>
        public MainWindowViewModel MainWindowViewModel { get; set; }

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

        #endregion

        public AccountListViewModel()
        {
            SearchCommand = new AppCommand(SearchAccount);
        }

        /// <summary>
        /// 执行搜索
        /// </summary>
        private async void SearchAccount()
        {
            if (MainWindowViewModel.ConnectionStatus != DatabaseConnectionStatus.Connected)
            {
                MainWindowViewModel.ShowErrorMessage("出错了", "数据库未连接");
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
                MainWindowViewModel.ShowErrorMessage("搜索出错", e.Message);
            }
        }

        private async Task<List<UserAccount>> DoSearchAccount()
        {
            var accountList = new List<UserAccount>();
            var mySqlConnection = MainWindowViewModel.MySqlConnection;
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
                var accountDbName = MainWindowViewModel.SelectedServer.AccountDbName;
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
    }
}