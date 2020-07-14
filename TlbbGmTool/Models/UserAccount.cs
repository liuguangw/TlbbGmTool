using TlbbGmTool.Core;

namespace TlbbGmTool.Models
{
    public class UserAccount : BindDataBase
    {
        #region Fields

        private int _id;
        private string _name = string.Empty;
        private string _password = string.Empty;
        private string _question = string.Empty;
        private string _answer = string.Empty;
        private string _email = string.Empty;
        private int _point;

        #endregion

        #region Properties

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string Question
        {
            get => _question;
            set => SetProperty(ref _question, value);
        }

        public string Answer
        {
            get => _answer;
            set => SetProperty(ref _answer, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public int Point
        {
            get => _point;
            set => SetProperty(ref _point, value);
        }

        #endregion
    }
}