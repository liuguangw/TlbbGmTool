namespace TlbbGmTool.Core
{
    public class ComboBoxNode<T>
    {
        public string Title { get; set; } = string.Empty;

        public T Value { get; set; } = default(T);
    }
}