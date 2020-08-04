using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class SelectEquipViewModel : BindDataBase
    {
        #region Fields

        private List<ItemBase> _equipBaseList;
        private SelectEquipWindow _selectEquipWindow;
        private ItemBase _selectedEquip;
        private bool _sameEquipPoint;
        private int _equipPoint = -1;
        private int _minLevel;
        private string _searchText = string.Empty;

        #endregion

        #region Properties

        public ItemBase SelectedEquip
        {
            get => _selectedEquip;
            set
            {
                if (SetProperty(ref _selectedEquip, value))
                {
                    ConfirmCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public List<ComboBoxNode<int>> EquipPointSelection { get; }

        public int EquipPoint
        {
            get => _equipPoint;
            set
            {
                if (SetProperty(ref _equipPoint, value))
                {
                    RaisePropertyChanged(nameof(FilterEquipList));
                }
            }
        }

        public int MinLevel
        {
            get => _minLevel;
            set
            {
                if (SetProperty(ref _minLevel, value))
                {
                    RaisePropertyChanged(nameof(FilterEquipList));
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    RaisePropertyChanged(nameof(FilterEquipList));
                }
            }
        }

        public Visibility SelectEquipPointVisibility =>
            _sameEquipPoint ? Visibility.Collapsed : Visibility.Visible;

        public List<ItemBase> FilterEquipList =>
            FilterEquip(_sameEquipPoint, _equipPoint, _minLevel, _searchText);

        public AppCommand ConfirmCommand { get; }
        public AppCommand CancelCommand { get; }

        #endregion

        public SelectEquipViewModel()
        {
            var equipPointSelection = new List<ComboBoxNode<int>>
            {
                new ComboBoxNode<int> {Title = "全部", Value = -1}
            };
            for (var i = 0; i <= 21; i++)
            {
                equipPointSelection.Add(new ComboBoxNode<int>
                {
                    Title = $"装备点{i}", Value = i
                });
            }

            EquipPointSelection = equipPointSelection;
            ConfirmCommand = new AppCommand(ConfirmSelect, CanConfirmSelect);
            CancelCommand = new AppCommand(CancelSelect);
        }


        public void InitData(List<ItemBase> equipBaseList, SelectEquipWindow selectEquipWindow, ItemBase equipBaseInfo,
            bool sameEquipPoint)
        {
            _equipBaseList = equipBaseList;
            _selectEquipWindow = selectEquipWindow;
            _sameEquipPoint = sameEquipPoint;
            RaisePropertyChanged(nameof(SelectEquipPointVisibility));
            SelectedEquip = equipBaseInfo;
            RaisePropertyChanged(nameof(FilterEquipList));
        }

        private List<ItemBase> FilterEquip(bool sameEquipPoint, int equipPoint, int minLevel, string searchText)
        {
            var targetEquipPoint = equipPoint;
            if (sameEquipPoint && _selectedEquip != null)
            {
                targetEquipPoint = _selectedEquip.EquipPoint;
            }

            return (from equipInfo in _equipBaseList
                where targetEquipPoint == -1 || targetEquipPoint == equipInfo.EquipPoint
                where equipInfo.Level >= minLevel
                where equipInfo.Name.IndexOf(searchText) >= 0
                select equipInfo).Take(60).ToList();
        }

        private bool CanConfirmSelect() => _selectedEquip != null;

        private void ConfirmSelect()
        {
            _selectEquipWindow.EquipBaseInfo = _selectedEquip;
            _selectEquipWindow.DialogResult = true;
            _selectEquipWindow.Close();
        }

        private void CancelSelect()
        {
            _selectEquipWindow.DialogResult = false;
            _selectEquipWindow.Close();
        }
    }
}