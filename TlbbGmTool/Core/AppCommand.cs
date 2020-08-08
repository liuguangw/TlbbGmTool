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

        private Action<object> _executeMethod;
        private Func<object, bool> _canExecuteMethod;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="executeMethod">执行功能的函数</param>
        /// <param name="canExecuteMethod">是否可以执行此功能</param>
        public AppCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
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
        public AppCommand(Action<object> executeMethod) : this(executeMethod,
            (_) => true)
        {
        }

        public AppCommand(Action executeMethod, Func<bool> canExecuteMethod) : this(_ => executeMethod(),
            _ => canExecuteMethod())
        {
        }


        public AppCommand(Action executeMethod) : this(executeMethod, () => true)
        {
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteMethod(parameter);
        }

        public void Execute(object parameter)
        {
            _executeMethod(parameter);
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