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
    public class BagItemListViewModel : BindDataBase
    {
        #region Fields

        private MainWindowViewModel _mainWindowViewModel;
        private EditRoleWindow _editRoleWindow;
        private int _charguid;

        #endregion

        #region Properties

        public ObservableCollection<ItemInfo> ItemList { get; } =
            new ObservableCollection<ItemInfo>();

        public AppCommand AddItemCommand { get; }

        public AppCommand EditItemCommand { get; }

        public AppCommand DeleteItemCommand { get; }

        #endregion

        public BagItemListViewModel()
        {
            AddItemCommand = new AppCommand(ShowAddDialog);
            EditItemCommand = new AppCommand(ShowEditDialog, CanEditItem);
            DeleteItemCommand = new AppCommand(ProcessDelete);
        }

        public void InitData(MainWindowViewModel mainWindowViewModel, int charguid, EditRoleWindow editRoleWindow)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _editRoleWindow = editRoleWindow;
            _charguid = charguid;

            // 每次切换到此页面时都重新加载
            LoadItemList();
        }

        private async void LoadItemList()
        {
            if (_mainWindowViewModel.ConnectionStatus != DatabaseConnectionStatus.Connected)
            {
                _mainWindowViewModel.ShowErrorMessage("出错了", "数据库未连接");
                return;
            }

            ItemList.Clear();
            try
            {
                var itemList = await DoLoadItemList();
                foreach (var item in itemList)
                {
                    ItemList.Add(item);
                }
            }
            catch (Exception e)
            {
                _mainWindowViewModel.ShowErrorMessage("加载出错", e.Message);
            }
        }


        private async Task<List<ItemInfo>> DoLoadItemList()
        {
            var itemList = new List<ItemInfo>();
            var mySqlConnection = _mainWindowViewModel.MySqlConnection;
            const int offset = 0;
            const int limit = 30;
            var sql =
                $"SELECT * FROM t_iteminfo WHERE charguid={_charguid}" +
                $" AND isvalid=1 AND pos>={offset}" +
                $" AND pos<{limit} ORDER BY pos ASC";
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            await Task.Run(async () =>
            {
                var gameDbName = _mainWindowViewModel.SelectedServer.GameDbName;
                if (mySqlConnection.Database != gameDbName)
                {
                    // 切换数据库
                    await mySqlConnection.ChangeDataBaseAsync(gameDbName);
                }

                var itemBases = _mainWindowViewModel.ItemBases;

                using (var rd = await mySqlCommand.ExecuteReaderAsync() as MySqlDataReader)
                {
                    while (await rd.ReadAsync())
                    {
                        var itemInfo = new ItemInfo(itemBases)
                        {
                            Aid = rd.GetInt32("aid"),
                            Charguid = rd.GetInt32("charguid"),
                            Guid = rd.GetInt32("guid"),
                            World = rd.GetInt32("world"),
                            Server = rd.GetInt32("server"),
                            ItemType = rd.GetInt32("itemtype"),
                            Pos = rd.GetInt32("pos"),
                            Creator = rd.GetString("creator")
                        };
                        var pArray = new int[17];
                        for (var i = 0; i < pArray.Length; i++)
                        {
                            pArray[i] = rd.GetInt32($"p{i + 1}");
                        }

                        itemInfo.PArray = pArray;
                        itemList.Add(itemInfo);
                    }
                }
            });
            return itemList;
        }

        private void ShowAddDialog()
        {
        }

        private bool CanEditItem(object parameter)
        {
            var itemInfo = parameter as ItemInfo;
            return !itemInfo.IsUnknownItem;
        }

        private void ShowEditDialog(object parameter)
        {
        }

        private void ProcessDelete(object parameter)
        {
        }
    }
}