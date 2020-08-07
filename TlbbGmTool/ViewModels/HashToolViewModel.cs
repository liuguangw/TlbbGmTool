using System.Security.Cryptography;
using System.Text;
using TlbbGmTool.Core;

namespace TlbbGmTool.ViewModels
{
    public class HashToolViewModel : BindDataBase
    {
        #region Fields

        private string _plainText = string.Empty;

        #endregion

        #region Properties

        public string PlainText
        {
            get => _plainText;
            set
            {
                if (SetProperty(ref _plainText, value))
                {
                    RaisePropertyChanged(nameof(HashText));
                }
            }
        }

        public string HashText => GetHashText(_plainText);

        #endregion

        private string GetHashText(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            var data = Encoding.UTF8.GetBytes(_plainText);
            byte[] hashData;
            using (var md5 = MD5.Create())
            {
                hashData = md5.ComputeHash(data);
            }

            var sBuilder = new StringBuilder();
            foreach (var t in hashData)
            {
                sBuilder.Append(t.ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}