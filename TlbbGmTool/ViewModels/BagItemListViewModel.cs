using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.Services;
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

        public AppCommand AddEquipCommand { get; }

        public AppCommand AddItemCommand { get; }
        public AppCommand EditItemCommand { get; }

        public AppCommand DeleteItemCommand { get; }

        #endregion

        public BagItemListViewModel()
        {
            AddEquipCommand = new AppCommand(ShowAddEquipDialog, CanAddItem);
            AddItemCommand = new AppCommand(ShowAddItemDialog, CanAddItem);
            EditItemCommand = new AppCommand(ShowEditDialog, CanEditItem);
            DeleteItemCommand = new AppCommand(ProcessDelete);
            ItemList.CollectionChanged += (sender, e) =>
            {
                AddEquipCommand.RaiseCanExecuteChanged();
                AddItemCommand.RaiseCanExecuteChanged();
            };
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
            var (startPos, endPos) = SaveItemService.GetBagItemIndexRange(SaveItemService.BagType.ItemBag);
            var sql =
                $"SELECT * FROM t_iteminfo WHERE charguid={_charguid}" +
                $" AND isvalid=1 AND pos>={startPos}" +
                $" AND pos<{endPos} ORDER BY pos ASC";
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

        private void ShowAddEquipDialog()
        {
            var editWindow = new AddOrEditEquipWindow(_mainWindowViewModel, null, _charguid, ItemList)
            {
                Owner = _editRoleWindow
            };
            editWindow.ShowDialog();
        }

        private void ShowAddItemDialog()
        {
            var editWindow = new AddOrEditItemWindow(_mainWindowViewModel, null,
                AddOrEditItemViewModel.ItemCategory.CommonItem, _charguid, ItemList)
            {
                Owner = _editRoleWindow
            };
            editWindow.ShowDialog();
        }

        private bool CanAddItem() => ItemList.Count < 30;

        private bool CanEditItem(object parameter)
        {
            var itemInfo = parameter as ItemInfo;
            return !itemInfo.IsUnknownItem;
        }

        private void ShowEditDialog(object parameter)
        {
            var itemInfo = parameter as ItemInfo;
            var itemBaseInfo = itemInfo.CurrentItemBase;
            if (itemBaseInfo == null)
            {
                return;
            }

            switch (itemBaseInfo.ItemClass)
            {
                //equip
                case 1:
                {
                    var editWindow = new AddOrEditEquipWindow(_mainWindowViewModel, itemInfo, _charguid, ItemList)
                    {
                        Owner = _editRoleWindow
                    };
                    editWindow.ShowDialog();
                    break;
                }
                //item
                case 3:
                {
                    var editWindow = new AddOrEditItemWindow(_mainWindowViewModel, itemInfo,
                        AddOrEditItemViewModel.ItemCategory.CommonItem, _charguid, ItemList)
                    {
                        Owner = _editRoleWindow
                    };
                    editWindow.ShowDialog();
                    break;
                }
                default:
                    _mainWindowViewModel.ShowErrorMessage("无法修改",
                        $"无法修改此类物品[ItemClass={itemBaseInfo.ItemClass}, ItemId={itemBaseInfo.Id}]");
                    break;
            }
        }

        private async void ProcessDelete(object parameter)
        {
            var itemInfo = parameter as ItemInfo;
            var tipName = $"{itemInfo.Name}(ID={itemInfo.ItemType}, Pos={itemInfo.Pos})";
            //删除确认
            if (MessageBox.Show(_editRoleWindow, $"确定要删除 {tipName}吗?",
                "删除提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                await DoProcessDelete(itemInfo.Charguid, itemInfo.Pos);
            }
            catch (Exception e)
            {
                _mainWindowViewModel.ShowErrorMessage("删除失败", e.Message);
                return;
            }

            //删除成功后,将itemInfo从列表移出
            ItemList.Remove(itemInfo);
            _mainWindowViewModel.ShowSuccessMessage("删除成功",
                $"删除 {tipName}成功");
        }

        private async Task DoProcessDelete(int charguid, int pos)
        {
            var sql = $"DELETE FROM t_iteminfo WHERE charguid={charguid} AND pos={pos}";
            var mySqlConnection = _mainWindowViewModel.MySqlConnection;
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            await Task.Run(async () =>
            {
                var gameDbName = _mainWindowViewModel.SelectedServer.GameDbName;
                if (mySqlConnection.Database != gameDbName)
                {
                    // 切换数据库
                    await mySqlConnection.ChangeDataBaseAsync(gameDbName);
                }

                await mySqlCommand.ExecuteNonQueryAsync();
            });
        }
    }
}