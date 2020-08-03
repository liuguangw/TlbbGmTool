using System;
using System.Collections.Generic;
using System.Linq;
using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class SelectGemViewModel : BindDataBase
    {
        #region Fields

        private List<ItemBase> _gemList;
        private SelectGemWindow _selectGemWindow;

        private int _gemId;
        private string _searchText = string.Empty;
        private int _level;

        #endregion

        #region Properties

        public List<ComboBoxNode<int>> LevelSelection { get; }

        public int GemId
        {
            get => _gemId;
            set => SetProperty(ref _gemId, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    RaisePropertyChanged(nameof(GemFilterList));
                }
            }
        }

        public int Level
        {
            get => _level;
            set
            {
                if (SetProperty(ref _level, value))
                {
                    RaisePropertyChanged(nameof(GemFilterList));
                }
            }
        }

        public List<ItemBase> GemFilterList => FilterGem(_level, _searchText);
        public AppCommand ConfirmCommand { get; }
        public AppCommand CancelCommand { get; }

        #endregion

        public SelectGemViewModel()
        {
            LevelSelection = new List<ComboBoxNode<int>>
            {
                new ComboBoxNode<int> {Title = "全部", Value = 0}
            };
            for (var i = 1; i <= 9; i++)
            {
                LevelSelection.Add(new ComboBoxNode<int>
                {
                    Title = $"{i}级",
                    Value = i
                });
            }

            RaisePropertyChanged(nameof(LevelSelection));

            ConfirmCommand = new AppCommand(ConfirmSelect, CanConfirmSelect);
            CancelCommand = new AppCommand(CancelSelect);
        }

        public void InitData(List<ItemBase> gemList, SelectGemWindow selectGemWindow, int gemId)
        {
            _gemList = gemList;
            _selectGemWindow = selectGemWindow;
            GemId = gemId;
            RaisePropertyChanged(nameof(GemFilterList));
        }


        private List<ItemBase> FilterGem(int level, string searchText)
        {
            if (_gemList == null)
            {
                return new List<ItemBase>();
            }

            return (from gemInfo in _gemList
                where level == 0 || gemInfo.Level == level
                where gemInfo.Name.IndexOf(searchText) >= 0
                select gemInfo).ToList();
        }

        private bool CanConfirmSelect() => _gemId != 0;

        private void ConfirmSelect()
        {
            _selectGemWindow.GemId = _gemId;
            _selectGemWindow.DialogResult = true;
            _selectGemWindow.Close();
        }

        private void CancelSelect()
        {
            _selectGemWindow.DialogResult = false;
            _selectGemWindow.Close();
        }
    }
}