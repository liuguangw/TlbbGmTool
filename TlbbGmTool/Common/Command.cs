using System;
using System.Windows.Input;

namespace liuguang.TlbbGmTool.Common;

public class Command : ICommand
{
    public event EventHandler? CanExecuteChanged;
    private Action<object?> _executeMethod;
    private Func<object?, bool>? _canExecuteMethod;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="executeMethod">执行功能的函数</param>
    /// <param name="canExecuteMethod">是否可以执行此功能</param>
    public Command(Action<object?> executeMethod, Func<object?, bool>? canExecuteMethod = null)
    {
        _executeMethod = executeMethod;
        _canExecuteMethod = canExecuteMethod;
    }

    public Command(Action executeMethod, Func<bool>? canExecuteMethod = null)
    {
        _executeMethod = parameter => executeMethod.Invoke();
        if (canExecuteMethod != null)
        {
            _canExecuteMethod = parameter => canExecuteMethod.Invoke();
        }
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecuteMethod?.Invoke(parameter) ?? true;
    }

    public void Execute(object? parameter)
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