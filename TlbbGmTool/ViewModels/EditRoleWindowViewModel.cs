using TlbbGmTool.Models;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class EditRoleWindowViewModel : GameRole
    {
        #region Fields

        private MainWindowViewModel _mainWindowViewModel;
        private GameRole _gameRole;
        private EditRoleWindow _editRoleWindow;

        #endregion

        public void InitData(MainWindowViewModel mainWindowViewModel, GameRole gameRole,
            EditRoleWindow editRoleWindow)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _gameRole = gameRole;
            _editRoleWindow = editRoleWindow;
            //初始化属性
            Aid = gameRole.Aid;
            Accname = gameRole.Accname;
            Charguid = gameRole.Charguid;
            Charname = gameRole.Charname;
            Title = gameRole.Title;
            Menpai = gameRole.Menpai;
            Level = gameRole.Level;
            Scene = gameRole.Scene;
            Xpos = gameRole.Xpos;
            Zpos = gameRole.Zpos;
            Hp = gameRole.Hp;
            Mp = gameRole.Mp;
            Str = gameRole.Str;
            Spr = gameRole.Spr;
            Con = gameRole.Con;
            Ipr = gameRole.Ipr;
            Dex = gameRole.Dex;
            Points = gameRole.Points;
            Enegry = gameRole.Enegry;
            Energymax = gameRole.Energymax;
            Vigor = gameRole.Vigor;
            Maxvigor = gameRole.Maxvigor;
            Exp = gameRole.Exp;
            Pkvalue = gameRole.Pkvalue;
            Vmoney = gameRole.Vmoney;
            Bankmoney = gameRole.Bankmoney;
            Yuanbao = gameRole.Yuanbao;
            Menpaipoint = gameRole.Menpaipoint;
            Zengdian = gameRole.Zengdian;
        }
    }
}