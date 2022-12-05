using System;
using System.Windows;

namespace liuguang.TlbbGmTool.Common;

public abstract class ViewModelBase : NotifyBase
{
    /// <summary>
    /// vm所属的窗体
    /// </summary>
    /// <value></value>
    public virtual Window? OwnedWindow { get; set; }


    /// <summary>
    /// 显示错误信息
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    protected void ShowErrorMessage(string title, string message)
    {
        ShowMessage(title, message, MessageBoxImage.Error);
    }

    /// <summary>
    /// 显示错误信息
    /// </summary>
    /// <param name="title"></param>
    /// <param name="ex"></param>
    /// <param name="withStackTrace"></param>
    protected void ShowErrorMessage(string title, Exception ex, bool withStackTrace = false)
    {
        ShowErrorMessage(title, withStackTrace ? $"{ex.Message}\n{ex.StackTrace}" : ex.Message);
    }

    /// <summary>
    /// 显示普通信息
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="icon"></param>
    protected void ShowMessage(string title, string message, MessageBoxImage icon = MessageBoxImage.Information)
    {
        MessageBox.Show(OwnedWindow, message, title, MessageBoxButton.OK, icon);
    }

    /// <summary>
    /// 操作确认
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    protected bool Confirm(string title, string message)
    {
        return MessageBox.Show(OwnedWindow, message, title,
        MessageBoxButton.YesNoCancel, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    /// <summary>
    /// 显示新的模态窗体
    /// </summary>
    /// <param name="dialogWindow"></param>
    /// <returns></returns>
    protected bool? ShowDialog(Window dialogWindow)
    {
        dialogWindow.Owner = OwnedWindow;
        return dialogWindow.ShowDialog();
    }

    /// <summary>
    /// 显示新的模态窗体,并为窗体绑定ViewModel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dialogWindow"></param>
    /// <param name="beforeAction"></param>
    /// <returns></returns>
    protected bool? ShowDialog<T>(Window dialogWindow, Action<T> beforeAction) where T : ViewModelBase
    {
        dialogWindow.Owner = OwnedWindow;
        var vm = (T)dialogWindow.DataContext;
        vm.OwnedWindow = dialogWindow;
        beforeAction.Invoke(vm);
        return dialogWindow.ShowDialog();
    }

}
