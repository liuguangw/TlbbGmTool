using TlbbGmTool.Core;
using TlbbGmTool.Models;
using TlbbGmTool.View.Windows;

namespace TlbbGmTool.ViewModels
{
    public class EditRoleWindowViewModel : BindDataBase
    {
        #region Fields

        public MainWindowViewModel MainWindowViewModel { get; private set; }
        public GameRole GameRole{ get; private set; }
        private EditRoleWindow _editRoleWindow;

        #endregion

        #region Properties

        public string WindowTitle => $"管理 {GameRole.Charname}(角色id: {GameRole.Charguid})";

        #endregion

        public void InitData(MainWindowViewModel mainWindowViewModel, GameRole gameRole,
            EditRoleWindow editRoleWindow)
        {
            MainWindowViewModel = mainWindowViewModel;
            GameRole = gameRole;
            _editRoleWindow = editRoleWindow;
        }

        /// <summary>
        /// 数据修改后,通知标题进行更新
        /// </summary>
        public void NotifyWindowTitleChange()
        {
            RaisePropertyChanged(nameof(WindowTitle));
        }
    }
}