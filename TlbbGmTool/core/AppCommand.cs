using System;
using System.Windows.Input;

namespace TlbbGmTool.Core
{
    /// <summary>
    /// 命令类
    /// </summary>
    public class AppCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action _executeMethod;
        private Func<bool> _canExecuteMethod;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="executeMethod">执行功能的函数</param>
        /// <param name="canExecuteMethod">是否可以执行此功能</param>
        public AppCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                throw new ArgumentNullException(nameof(executeMethod));
            }

            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="executeMethod">执行功能的函数</param>
        public AppCommand(Action executeMethod) : this(executeMethod, () => true)
        {

        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteMethod();
        }

        public void Execute(object parameter)
        {
            _executeMethod();
        }

        /// <summary>
        /// 发送可执行状态变化事件
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}
