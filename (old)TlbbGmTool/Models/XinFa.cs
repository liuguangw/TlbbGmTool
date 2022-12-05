using TlbbGmTool.Core;

namespace TlbbGmTool.Models
{
    public class XinFa : BindDataBase
    {
        #region Fields

        private int _aid;
        private int _charguid;
        private int _xinfaid;
        private int _xinfalvl;

        #endregion

        #region Properties

        public int Aid
        {
            get => _aid;
            set => SetProperty(ref _aid, value);
        }

        public int Charguid
        {
            get => _charguid;
            set => SetProperty(ref _charguid, value);
        }

        public int Xinfaid
        {
            get => _xinfaid;
            set => SetProperty(ref _xinfaid, value);
        }

        public int Xinfalvl
        {
            get => _xinfalvl;
            set => SetProperty(ref _xinfalvl, value);
        }

        #endregion
    }
}