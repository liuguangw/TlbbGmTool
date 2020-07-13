using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TlbbGmTool.Core
{
    /// <summary>
    /// 数据绑定功能基础类
    /// </summary>
    public abstract class BindDataBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 处理属性变化事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 设置属性,如果属性发生变化则返回true
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="storage">存储值引用</param>
        /// <param name="value">新值</param>
        /// <param name="propertyName">属性名</param>
        /// <returns>属性是否变化</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// 发送属性变化事件
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
