using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.Services;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class AddOrEditEquipViewModel : BindDataBase
    {
        #region Fields

        private MainWindowViewModel _mainWindowViewModel;
        private ItemInfo _itemInfo;
        private int _charguid;
        private bool _isAddEquip = true;
        private AddOrEditEquipWindow _addOrEditEquipWindow;

        //
        private List<ItemBase> _equipBaseList = new List<ItemBase>();
        private List<ItemBase> _gemBaseList = new List<ItemBase>();
        private int _itemBaseId;
        private int _starCount = 0;
        private int _slotCount = 0;
        private int _enhanceCount = 0;
        private int _floatValue = 0;
        private int _attr1 = 0;

        private int _attr2 = 0;

        //
        private int _gem1;
        private int _gem2;
        private int _gem3;

        private int _gem4;

        //
        private int _qualification1;
        private int _qualification2;
        private int _qualification3;
        private int _qualification4;
        private int _qualification5;

        private int _qualification6;

        //
        private bool _bindStatus;
        private bool _verifiedStatus;
        private bool _qualificationVerifiedStatus;
        private bool _engravedStatus;
        private int _equipVisual;

        #endregion

        #region Properties

        public string WindowTitle => _isAddEquip ? "添加装备" : $"修改装备 {ItemName})";

        public int ItemBaseId
        {
            set
            {
                if (SetProperty(ref _itemBaseId, value))
                {
                    RaisePropertyChanged(nameof(ItemName));
                    RaisePropertyChanged(nameof(WindowTitle));
                }
            }
        }

        public string ItemName => FindItemName(_itemBaseId, _equipBaseList);

        public List<ComboBoxNode<int>> StarSection { get; } = new List<ComboBoxNode<int>>();

        public int StarCount
        {
            get => _starCount;
            set => SetProperty(ref _starCount, value);
        }

        public List<ComboBoxNode<int>> SlotSelection { get; } = new List<ComboBoxNode<int>>();

        public int SlotCount
        {
            get => _slotCount;
            set
            {
                if (!SetProperty(ref _slotCount, value))
                {
                    return;
                }

                SelectGem1Command.RaiseCanExecuteChanged();
                SelectGem2Command.RaiseCanExecuteChanged();
                SelectGem3Command.RaiseCanExecuteChanged();
                SelectGem4Command.RaiseCanExecuteChanged();
            }
        }

        public int EnhanceCount
        {
            get => _enhanceCount;
            set => SetProperty(ref _enhanceCount, value);
        }

        public int FloatValue
        {
            get => _floatValue;
            set => SetProperty(ref _floatValue, value);
        }

        public int Attr1
        {
            set
            {
                if (SetProperty(ref _attr1, value))
                {
                    RaisePropertyChanged(nameof(Attr1Tip));
                }
            }
        }

        public string Attr1Tip => CalcAttrTip(_attr1);

        public int Attr2
        {
            set
            {
                if (SetProperty(ref _attr2, value))
                {
                    RaisePropertyChanged(nameof(Attr2Tip));
                }
            }
        }

        public string Attr2Tip => CalcAttrTip(_attr2);

        public int Gem1
        {
            set
            {
                if (SetProperty(ref _gem1, value))
                {
                    RaisePropertyChanged(nameof(Gem1Name));
                }
            }
        }

        public int Gem2
        {
            set
            {
                if (SetProperty(ref _gem2, value))
                {
                    RaisePropertyChanged(nameof(Gem2Name));
                }
            }
        }

        public int Gem3
        {
            set
            {
                if (SetProperty(ref _gem3, value))
                {
                    RaisePropertyChanged(nameof(Gem3Name));
                }
            }
        }

        public int Gem4
        {
            set
            {
                if (SetProperty(ref _gem4, value))
                {
                    RaisePropertyChanged(nameof(Gem4Name));
                }
            }
        }

        public string Gem1Name => FindItemName(_gem1, _gemBaseList);

        public string Gem2Name => FindItemName(_gem2, _gemBaseList);

        public string Gem3Name => FindItemName(_gem3, _gemBaseList);

        public string Gem4Name => FindItemName(_gem4, _gemBaseList);

        public int Qualification1
        {
            get => _qualification1;
            set => SetProperty(ref _qualification1, value);
        }

        public int Qualification2
        {
            get => _qualification2;
            set => SetProperty(ref _qualification2, value);
        }

        public int Qualification3
        {
            get => _qualification3;
            set => SetProperty(ref _qualification3, value);
        }

        public int Qualification4
        {
            get => _qualification4;
            set => SetProperty(ref _qualification4, value);
        }

        public int Qualification5
        {
            get => _qualification5;
            set => SetProperty(ref _qualification5, value);
        }

        public int Qualification6
        {
            get => _qualification6;
            set => SetProperty(ref _qualification6, value);
        }

        public bool BindStatus
        {
            get => _bindStatus;
            set => SetProperty(ref _bindStatus, value);
        }

        public bool VerifiedStatus
        {
            get => _verifiedStatus;
            set => SetProperty(ref _verifiedStatus, value);
        }

        public bool QualificationVerifiedStatus
        {
            get => _qualificationVerifiedStatus;
            set => SetProperty(ref _qualificationVerifiedStatus, value);
        }

        public bool EngravedStatus
        {
            get => _engravedStatus;
            set => SetProperty(ref _engravedStatus, value);
        }

        public int EquipVisual
        {
            set
            {
                if (SetProperty(ref _equipVisual, value))
                {
                    RaisePropertyChanged(nameof(EquipVisualName));
                }
            }
        }

        public string EquipVisualName
        {
            get
            {
                if (_equipVisual == 0)
                {
                    return "未知";
                }

                var itemBaseInfo = (from baseInfo in _equipBaseList
                    where baseInfo.EquipVisual == _equipVisual
                    select baseInfo).First();
                return itemBaseInfo != null
                    ? $"{itemBaseInfo.Name}(ID: {itemBaseInfo.Id})"
                    : $"未知(Visual: {_equipVisual})";
            }
        }

        #endregion

        #region Commands

        public AppCommand SaveEquipCommand { get; }

        public AppCommand SelectGem1Command { get; }

        public AppCommand SelectGem2Command { get; }

        public AppCommand SelectGem3Command { get; }

        public AppCommand SelectGem4Command { get; }

        public AppCommand SelectEquipCommand { get; }

        public AppCommand SelectVisualCommand { get; }

        public AppCommand SelectAttrCommand { get; }

        #endregion

        public AddOrEditEquipViewModel()
        {
            SaveEquipCommand = new AppCommand(SaveEquip);
            for (var i = 0; i <= 9; i++)
            {
                StarSection.Add(new ComboBoxNode<int>()
                {
                    Title = $"{i}星",
                    Value = i
                });
                if (i <= 4)
                {
                    SlotSelection.Add(new ComboBoxNode<int>()
                    {
                        Title = $"{i}孔",
                        Value = i
                    });
                }
            }

            SelectGem1Command = new AppCommand(
                () => SelectGem(
                    _gem1,
                    value => Gem1 = value
                ), () => _slotCount >= 1);
            SelectGem2Command = new AppCommand(
                () => SelectGem(
                    _gem2,
                    value => Gem2 = value
                ), () => _slotCount >= 2);
            SelectGem3Command = new AppCommand(
                () => SelectGem(
                    _gem3,
                    value => Gem3 = value
                ), () => _slotCount >= 3);
            SelectGem4Command = new AppCommand(
                () => SelectGem(
                    _gem4,
                    value => Gem4 = value
                ), () => _slotCount >= 4);
            SelectEquipCommand = new AppCommand(SelectEquip);
            SelectVisualCommand = new AppCommand(SelectVisual);
            SelectAttrCommand = new AppCommand(SelectAttr);
        }

        public void InitData(MainWindowViewModel mainWindowViewModel, ItemInfo itemInfo,
            int charguid, AddOrEditEquipWindow addOrEditEquipWindow)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _addOrEditEquipWindow = addOrEditEquipWindow;
            _equipBaseList = (from itemBaseInfoPair in _mainWindowViewModel.ItemBases
                where itemBaseInfoPair.Value.ItemClass == 1
                select itemBaseInfoPair.Value).ToList();
            _gemBaseList = (from itemBaseInfoPair in _mainWindowViewModel.ItemBases
                where itemBaseInfoPair.Value.ItemClass == 5
                select itemBaseInfoPair.Value).ToList();
            _charguid = charguid;
            if (itemInfo == null)
            {
                //初始化默认值
                var firstItem = _equipBaseList.First();
                ItemBaseId = firstItem.Id;
                EquipVisual = firstItem.Id;
                return;
            }

            _itemInfo = itemInfo;
            _isAddEquip = false;
            //初始化属性
            LoadEquipInfo();
        }

        /// <summary>
        /// 从itemInfo中加载equip信息
        /// </summary>
        private void LoadEquipInfo()
        {
            ItemBaseId = _itemInfo.ItemType;
            StarCount = (_itemInfo.PArray[8] >> 8) & 0xff;
            SlotCount = _itemInfo.PArray[4] & 0xff;
            EquipVisual = (_itemInfo.PArray[8] >> 16) & 0xffff;
            Gem1 = ((_itemInfo.PArray[1] & 0xffff) << 16) + (_itemInfo.PArray[0] >> 16);
            Gem2 = ((_itemInfo.PArray[2] & 0xffff) << 16) + (_itemInfo.PArray[1] >> 16);
            Gem3 = ((_itemInfo.PArray[3] & 0xffff) << 16) + (_itemInfo.PArray[2] >> 16);
            Gem4 = ((_itemInfo.PArray[16] & 0xff) << 24) + ((_itemInfo.PArray[15] >> 8) & 0xffffff);
            Attr1 = _itemInfo.PArray[9];
            Attr2 = _itemInfo.PArray[10];
            EnhanceCount = _itemInfo.PArray[5] >> 24;
            FloatValue = _itemInfo.PArray[11] & 0xff;
            Qualification1 = (_itemInfo.PArray[6] >> 24) & 0xff;
            Qualification2 = (_itemInfo.PArray[7] >> 8) & 0xff;
            Qualification3 = _itemInfo.PArray[7] & 0xff;
            Qualification4 = (_itemInfo.PArray[7] >> 16) & 0xff;
            Qualification5 = (_itemInfo.PArray[7] >> 24) & 0xff;
            Qualification6 = _itemInfo.PArray[8] & 0xff;
            var statusVal = (_itemInfo.PArray[3] >> 16) & 0xff;
            BindStatus = (statusVal & 1) != 0;
            VerifiedStatus = ((statusVal >> 1) & 1) != 0;
            QualificationVerifiedStatus = ((statusVal >> 5) & 1) != 0;
            EngravedStatus = ((statusVal >> 6) & 1) != 0;
        }

        private async void SaveEquip()
        {
            ItemInfo itemInfo;
            try
            {
                itemInfo = await DoSaveEquip();
            }
            catch (Exception e)
            {
                _mainWindowViewModel.ShowErrorMessage("保存失败", e.Message);
                return;
            }

            //更新属性
            if (_itemInfo != null)
            {
                _itemInfo.ItemType = itemInfo.ItemType;
                _itemInfo.PArray = itemInfo.PArray;
                _itemInfo.Creator = itemInfo.Creator;
            }

            //更新标题
            RaisePropertyChanged(nameof(WindowTitle));
            _mainWindowViewModel.ShowSuccessMessage("保存成功", "保存equip信息成功");
            _addOrEditEquipWindow.Close();
        }

        private async Task<ItemInfo> DoSaveEquip()
        {
            var attrCount = GetAttrCountFromNumber(_attr1)
                            + GetAttrCountFromNumber(_attr2);
            var itemType = _itemBaseId;
            var gemCount = 0;
            var pArray = new int[17];
            if (_gem1 != 0)
            {
                gemCount++;
                //Gem1前两字节 -> p2后两字节
                pArray[1] = (int) (pArray[1] & 0xffff0000);
                pArray[1] |= (_gem1 >> 16) & 0xffff;
                //Gem1后两字节 -> p1前两字节
                pArray[0] &= 0xffff;
                pArray[0] |= (_gem1 & 0xffff) << 16;
            }

            if (_gem2 != 0)
            {
                gemCount++;
                //Gem2前两字节 -> p3后两字节
                pArray[2] = (int) (pArray[2] & 0xffff0000);
                pArray[2] |= (_gem2 >> 16) & 0xffff;
                //Gem2后两字节 -> p2前两字节
                pArray[1] &= 0xffff;
                pArray[1] |= (_gem2 & 0xffff) << 16;
            }

            if (_gem3 != 0)
            {
                gemCount++;
                //Gem3前两字节 -> p4后两字节
                pArray[3] = (int) (pArray[1] & 0xffff0000);
                pArray[3] |= (_gem3 >> 16) & 0xffff;
                //Gem3后两字节 -> p3前两字节
                pArray[2] &= 0xffff;
                pArray[2] |= (_gem3 & 0xffff) << 16;
            }

            if (_gem4 != 0)
            {
                gemCount++;
                //Gem4首字节 -> p17尾字节
                pArray[16] = (int) (pArray[16] & 0xffffff00);
                pArray[16] |= _gem4 >> 24;
                //Gem4后三字节 -> p16前三字节
                pArray[15] = pArray[15] & 0xff;
                pArray[15] += (_gem4 & 0xffffff) << 8;
            }

            //取状态信息(P4第二个字节),将对应位置重置为0
            var itemStatus = (pArray[3] >> 16) & 0xff;
            itemStatus = itemStatus & 0b10011100;
            if (_bindStatus)
            {
                itemStatus |= 1;
            }

            if (_verifiedStatus)
            {
                itemStatus |= 1 << 1;
            }

            if (_qualificationVerifiedStatus)
            {
                itemStatus |= 1 << 5;
            }

            if (_engravedStatus)
            {
                itemStatus |= 1 << 6;
            }

            //替换状态数据
            pArray[3] = (int) (pArray[3] & 0xff00ffff);
            pArray[3] |= itemStatus << 16;
            //Slot Count
            pArray[4] = (int) (pArray[4] & 0xffffff00);
            pArray[4] |= SlotCount;
            //P6 前三字节置0
            pArray[5] = pArray[5] & 0xff;
            //填充数据
            pArray[5] |= attrCount << 8;
            pArray[5] |= gemCount << 16;
            pArray[5] |= _enhanceCount << 24;
            //quality
            pArray[6] = pArray[6] & 0xffffff;
            pArray[6] |= _qualification1 << 24;
            pArray[7] = 0;
            pArray[7] |= _qualification5 << 24;
            pArray[7] |= _qualification4 << 16;
            pArray[7] |= _qualification2 << 8;
            pArray[7] |= _qualification3;
            pArray[8] = _equipVisual << 16;
            pArray[8] |= _starCount << 8;
            pArray[8] |= _qualification6;
            //attr
            pArray[9] = _attr1;
            pArray[10] = _attr2;
            //float value
            pArray[11] = (int) (pArray[11] & 0xffffff00);
            pArray[11] += FloatValue;
            //enhance
            pArray[12] = (int) (pArray[12] & 0xffffff00);
            pArray[12] += EnhanceCount;
            ItemInfo itemInfo;
            if (_isAddEquip)
            {
                //@todo insert into list
                itemInfo = await DoInsertEquip(itemType, pArray, _charguid);
            }
            else
            {
                itemInfo = await DoUpdateEquip(itemType, pArray, _charguid, _itemInfo.Pos);
            }

            return itemInfo;
        }

        private async Task<MySqlConnection> GetMySqlConnection()
        {
            var mySqlConnection = _mainWindowViewModel.MySqlConnection;
            var gameDbName = _mainWindowViewModel.SelectedServer.GameDbName;
            if (mySqlConnection.Database != gameDbName)
            {
                // 切换数据库
                await mySqlConnection.ChangeDataBaseAsync(gameDbName);
            }

            return mySqlConnection;
        }

        private async Task<ItemInfo> DoUpdateEquip(int itemType, int[] pArray, int charguid, int pos)
        {
            var sql = "UPDATE t_iteminfo SET itemtype=@itemtype";
            var intDictionary = new Dictionary<string, int>();
            for (var i = 0; i < pArray.Length; i++)
            {
                intDictionary[$"p{i + 1}"] = pArray[i];
            }

            foreach (var keyValuePair in intDictionary)
            {
                sql += $",{keyValuePair.Key}=@{keyValuePair.Key}";
            }

            sql += " WHERE charguid=@charguid AND pos=@pos";
            intDictionary.Add("itemtype", itemType);
            intDictionary.Add("charguid", charguid);
            intDictionary.Add("pos", pos);
            //构造参数
            var mySqlParameters = (from intParameter in intDictionary
                select new MySqlParameter("@" + intParameter.Key, MySqlDbType.Int32)
                {
                    Value = intParameter.Value
                }).ToList();
            var mySqlConnection = await GetMySqlConnection();
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            mySqlParameters.ForEach(mySqlParameter => mySqlCommand.Parameters.Add(mySqlParameter));
            await Task.Run(async () => { await mySqlCommand.ExecuteNonQueryAsync(); });
            return new ItemInfo(_mainWindowViewModel.ItemBases)
            {
                ItemType = itemType,
                PArray = pArray,
                Creator = _itemInfo.Creator
            };
        }

        /// <summary>
        /// 获取下一个有效的pos
        /// </summary>
        /// <param name="mySqlConnection"></param>
        /// <param name="charguid"></param>
        /// <returns></returns>
        private async Task<int> GetNextPos(MySqlConnection mySqlConnection, int charguid)
        {
            var currentPos = 0;
            var findPos = false;
            var sql = "SELECT pos FROM t_iteminfo WHERE charguid="
                      + charguid + " AND isvalid=1"
                      + " AND pos>=0 AND pos<30"
                      + " ORDER BY pos ASC";
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            await Task.Run(async () =>
            {
                using (var rd = await mySqlCommand.ExecuteReaderAsync() as MySqlDataReader)
                {
                    while (await rd.ReadAsync())
                    {
                        var pos = rd.GetInt32("pos");
                        if (currentPos < pos)
                        {
                            findPos = true;
                            break;
                        }

                        currentPos++;
                    }
                }
            });
            if (!findPos)
            {
                throw new Exception("找不到有效的pos");
            }

            //清理pos
            sql = "DELETE FROM t_iteminfo WHERE charguid=" + charguid + " AND pos=" + currentPos;
            mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            await Task.Run(async () => { await mySqlCommand.ExecuteNonQueryAsync(); });
            return currentPos;
        }

        private async Task<int> GetNextGuid(MySqlConnection mySqlConnection)
        {
            var nextGuid = 0;
            const string sql = "SELECT serial FROM t_itemkey WHERE smkey=7001 LIMIT 1";
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            await Task.Run(async () =>
            {
                using (var rd = await mySqlCommand.ExecuteReaderAsync() as MySqlDataReader)
                {
                    nextGuid = rd.GetInt32("serial");
                }

                //UPDATE KEY
                var updateSql = "UPDATE t_itemkey SET serial=serial+2 WHERE smkey=7001";
                mySqlCommand = new MySqlCommand(updateSql, mySqlConnection);
                await mySqlCommand.ExecuteNonQueryAsync();
            });
            return nextGuid + 2;
        }

        private async Task<ItemInfo> DoInsertEquip(int itemType, int[] pArray, int charguid)
        {
            var mySqlConnection = await GetMySqlConnection();
            var pos = await GetNextPos(mySqlConnection, charguid);
            var guid = await GetNextGuid(mySqlConnection);
            var creator = "test";
            //最大耐久值
            var equipLife = 0xEB;
            pArray[3] = pArray[3] & 0xffffff;
            pArray[3] |= equipLife << 24;
            pArray[4] = (int) (pArray[3] & 0xff00ffff);
            pArray[4] |= equipLife << 16;
            var intDictionary = new Dictionary<string, int>()
            {
                ["charguid"] = charguid,
                ["guid"] = guid,
                ["world"] = 101,
                ["server"] = 0,
                ["itemtype"] = itemType,
                ["pos"] = pos,
            };
            for (var i = 0; i < pArray.Length; i++)
            {
                intDictionary.Add($"p{i + 1}", pArray[i]);
            }

            var fieldNames = intDictionary.Keys.ToList();
            fieldNames.AddRange(new[]
            {
                "creator",
                "fixattr",
                "var"
            });
            var sql = "INSERT INTO t_iteminfo";
            sql += "(" + string.Join(", ", fieldNames) + ") VALUES";
            var fieldValueTemplates = (from fieldName in fieldNames
                select "@" + fieldName);
            sql += " (" + string.Join(", ", fieldValueTemplates) + ")";
            //构造参数
            var mySqlParameters = (from intParameter in intDictionary
                select new MySqlParameter("@" + intParameter.Key, MySqlDbType.Int32)
                {
                    Value = intParameter.Value
                }).ToList();
            mySqlParameters.Add(new MySqlParameter("@creator", MySqlDbType.String)
            {
                Value = DbStringService.ToDbString("流光")
            });
            mySqlParameters.Add(new MySqlParameter("@fixattr", MySqlDbType.String)
            {
                Value = string.Empty
            });
            mySqlParameters.Add(new MySqlParameter("@var", MySqlDbType.String)
            {
                Value = string.Empty
            });
            var mySqlCommand = new MySqlCommand(sql, mySqlConnection);
            mySqlParameters.ForEach(mySqlParameter => mySqlCommand.Parameters.Add(mySqlParameter));
            await Task.Run(async () => { await mySqlCommand.ExecuteNonQueryAsync(); });
            return new ItemInfo(_mainWindowViewModel.ItemBases)
            {
                Charguid = charguid,
                Guid = guid,
                World = intDictionary["world"],
                Server = intDictionary["server"],
                ItemType = itemType,
                Pos = intDictionary["pos"],
                PArray = pArray
            };
        }

        /// <summary>
        /// 通过ID查找名称
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemBaseList"></param>
        /// <returns></returns>
        private static string FindItemName(int itemId, IEnumerable<ItemBase> itemBaseList)
        {
            if (itemId == 0)
            {
                return "无";
            }

            var itemBaseInfo = (from baseInfo in itemBaseList
                where baseInfo.Id == itemId
                select baseInfo).First();
            return itemBaseInfo != null ? $"{itemBaseInfo.Name}(ID: {itemId})" : $"未知(ID: {itemId})";
        }

        private static int GetAttrCountFromNumber(int attrValue)
        {
            var attrCount = 0;
            for (var i = 0; i < 32; i++)
            {
                var tmpValue = attrValue;
                if (i > 0)
                {
                    tmpValue >>= i;
                }

                if ((tmpValue & 1) != 0)
                {
                    attrCount++;
                }
            }

            return attrCount;
        }

        private static string CalcAttrTip(int attrValue)
            => $"已选择{GetAttrCountFromNumber(attrValue)}种属性";

        private void SelectGem(int gemId, Action<int> setGemCallback)
        {
            var selectGemWindow = new SelectGemWindow(_gemBaseList, gemId)
            {
                Owner = _addOrEditEquipWindow
            };
            if (selectGemWindow.ShowDialog() == true)
            {
                setGemCallback(selectGemWindow.GemId);
            }
        }

        private void SelectEquip()
        {
            var selectEquipWindow = new SelectEquipWindow(_equipBaseList, _itemBaseId)
            {
                Owner = _addOrEditEquipWindow
            };
            if (selectEquipWindow.ShowDialog() == true)
            {
                var selectedEquip = selectEquipWindow.EquipBaseInfo;
                ItemBaseId = selectedEquip.Id;
                EquipVisual = selectedEquip.EquipVisual;
            }
        }

        private void SelectVisual()
        {
            var selectEquipWindow = new SelectEquipWindow(_equipBaseList, _itemBaseId, true)
            {
                Owner = _addOrEditEquipWindow
            };
            if (selectEquipWindow.ShowDialog() == true)
            {
                var selectedEquip = selectEquipWindow.EquipBaseInfo;
                EquipVisual = selectedEquip.EquipVisual;
            }
        }

        private void SelectAttr()
        {
            var selectAttrWindow = new SelectAttrWindow(_equipBaseList, _itemBaseId, _attr1, _attr2)
            {
                Owner = _addOrEditEquipWindow
            };
            if (selectAttrWindow.ShowDialog() == true)
            {
                Attr1 = selectAttrWindow.Attr1;
                Attr2 = selectAttrWindow.Attr2;
            }
        }
    }
}