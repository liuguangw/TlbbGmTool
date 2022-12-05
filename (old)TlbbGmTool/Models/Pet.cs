using TlbbGmTool.Core;

namespace TlbbGmTool.Models
{
    public class Pet : BindDataBase
    {
        #region Fields

        private int _petGuid;
        private int _charguid;
        private string _petName = string.Empty;
        private int _level;
        private int _needLevel;
        private int _aiType;
        private int _life;
        private int _petType;
        private int _genera;

        private int _enjoy;

        //
        private int _strper;
        private int _conper;
        private int _dexper;
        private int _sprper;
        private int _iprper;

        //
        private int _savvy;
        private int _gengu;
        private int _growrate;
        private int _repoint;
        private int _exp;

        //
        private int _str;
        private int _con;
        private int _dex;
        private int _spr;

        private int _ipr;

        //
        private string _skill = string.Empty;

        #endregion

        #region Properties

        public int PetGuid
        {
            get => _petGuid;
            set => SetProperty(ref _petGuid, value);
        }

        public int Charguid
        {
            get => _charguid;
            set => SetProperty(ref _charguid, value);
        }

        public string PetName
        {
            get => _petName;
            set => SetProperty(ref _petName, value);
        }

        public int Level
        {
            get => _level;
            set => SetProperty(ref _level, value);
        }

        public int NeedLevel
        {
            get => _needLevel;
            set => SetProperty(ref _needLevel, value);
        }

        public int AiType
        {
            get => _aiType;
            set => SetProperty(ref _aiType, value);
        }

        public int Life
        {
            get => _life;
            set => SetProperty(ref _life, value);
        }

        public int PetType
        {
            get => _petType;
            set => SetProperty(ref _petType, value);
        }

        public int Genera
        {
            get => _genera;
            set => SetProperty(ref _genera, value);
        }

        public int Enjoy
        {
            get => _enjoy;
            set => SetProperty(ref _enjoy, value);
        }

        public int Strper
        {
            get => _strper;
            set => SetProperty(ref _strper, value);
        }

        public int Conper
        {
            get => _conper;
            set => SetProperty(ref _conper, value);
        }

        public int Dexper
        {
            get => _dexper;
            set => SetProperty(ref _dexper, value);
        }

        public int Sprper
        {
            get => _sprper;
            set => SetProperty(ref _sprper, value);
        }

        public int Iprper
        {
            get => _iprper;
            set => SetProperty(ref _iprper, value);
        }

        public int Savvy
        {
            get => _savvy;
            set => SetProperty(ref _savvy, value);
        }

        public int Gengu
        {
            get => _gengu;
            set => SetProperty(ref _gengu, value);
        }

        public int Growrate
        {
            get => _growrate;
            set => SetProperty(ref _growrate, value);
        }

        public int Repoint
        {
            get => _repoint;
            set => SetProperty(ref _repoint, value);
        }

        public int Exp
        {
            get => _exp;
            set => SetProperty(ref _exp, value);
        }

        public int Str
        {
            get => _str;
            set => SetProperty(ref _str, value);
        }

        public int Con
        {
            get => _con;
            set => SetProperty(ref _con, value);
        }

        public int Dex
        {
            get => _dex;
            set => SetProperty(ref _dex, value);
        }

        public int Spr
        {
            get => _spr;
            set => SetProperty(ref _spr, value);
        }

        public int Ipr
        {
            get => _ipr;
            set => SetProperty(ref _ipr, value);
        }

        public string Skill
        {
            get => _skill;
            set => SetProperty(ref _skill, value);
        }

        #endregion
    }
}