namespace liuguang.TlbbGmTool.Common;

/// <summary>
/// 下拉框选项
/// </summary>
/// <typeparam name="T"></typeparam>
public class ComboBoxNode<T>
{
    public string Title { get; }

    public T Value { get; }

    public ComboBoxNode(string title, T value)
    {
        Title = title;
        Value = value;
    }
}
