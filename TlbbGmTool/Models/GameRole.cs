using System.Collections.Generic;
using TlbbGmTool.Core;

namespace TlbbGmTool.Models
{
    public class GameRole : BindDataBase
    {
        public static Dictionary<int, string> MenpaiList = new Dictionary<int, string>();

        #region Fields

        private string _accname = string.Empty;
        private int _charguid;
        private string _charname = string.Empty;
        private string _title = string.Empty;
        private int _menpai;

        private int _level;

        //
        private int _scene;
        private int _xpos;

        private int _zpos;

        //
        private int _hp;

        private int _mp;

        //
        private int _str;
        private int _spr;
        private int _con;
        private int _ipr;
        private int _dex;

        private int _points;

        //
        private int _enegry;
        private int _energymax;
        private int _vigor;

        private int _maxvigor;

        //
        private int _exp;
        private int _pkvalue;
        private int _vmoney;
        private int _bankmoney;
        private int _yuanbao;
        private int _menpaipoint;
        private int _zengdian;

        #endregion

        #region Properties

        public string Accname
        {
            get => _accname;
            set => SetProperty(ref _accname, value);
        }

        public int Charguid
        {
            get => _charguid;
            set => SetProperty(ref _charguid, value);
        }

        public string Charname
        {
            get => _charname;
            set => SetProperty(ref _charname, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public int Menpai
        {
            get => _menpai;
            set
            {
                if (SetProperty(ref _menpai, value))
                {
                    RaisePropertyChanged(nameof(MenpaiTip));
                }
            }
        }

        public string MenpaiTip =>
            MenpaiList.ContainsKey(_menpai) ? MenpaiList[_menpai] : "-";

        public int Level
        {
            get => _level;
            set => SetProperty(ref _level, value);
        }

        public int Scene
        {
            get => _scene;
            set => SetProperty(ref _scene, value);
        }

        public int Xpos
        {
            get => _xpos;
            set => SetProperty(ref _xpos, value);
        }

        public int Zpos
        {
            get => _zpos;
            set => SetProperty(ref _zpos, value);
        }

        public int Hp
        {
            get => _hp;
            set => SetProperty(ref _hp, value);
        }

        public int Mp
        {
            get => _mp;
            set => SetProperty(ref _mp, value);
        }

        public int Str
        {
            get => _str;
            set => SetProperty(ref _str, value);
        }

        public int Spr
        {
            get => _spr;
            set => SetProperty(ref _spr, value);
        }

        public int Con
        {
            get => _con;
            set => SetProperty(ref _con, value);
        }

        public int Ipr
        {
            get => _ipr;
            set => SetProperty(ref _ipr, value);
        }

        public int Dex
        {
            get => _dex;
            set => SetProperty(ref _dex, value);
        }

        public int Points
        {
            get => _points;
            set => SetProperty(ref _points, value);
        }

        public int Enegry
        {
            get => _enegry;
            set => SetProperty(ref _enegry, value);
        }

        public int Energymax
        {
            get => _energymax;
            set => SetProperty(ref _energymax, value);
        }

        public int Vigor
        {
            get => _vigor;
            set => SetProperty(ref _vigor, value);
        }

        public int Maxvigor
        {
            get => _maxvigor;
            set => SetProperty(ref _maxvigor, value);
        }

        public int Exp
        {
            get => _exp;
            set => SetProperty(ref _exp, value);
        }

        public int Pkvalue
        {
            get => _pkvalue;
            set => SetProperty(ref _pkvalue, value);
        }

        public int Vmoney
        {
            get => _vmoney;
            set => SetProperty(ref _vmoney, value);
        }

        public int Bankmoney
        {
            get => _bankmoney;
            set => SetProperty(ref _bankmoney, value);
        }

        public int Yuanbao
        {
            get => _yuanbao;
            set => SetProperty(ref _yuanbao, value);
        }

        public int Menpaipoint
        {
            get => _menpaipoint;
            set => SetProperty(ref _menpaipoint, value);
        }

        public int Zengdian
        {
            get => _zengdian;
            set => SetProperty(ref _zengdian, value);
        }

        #endregion
    }
}