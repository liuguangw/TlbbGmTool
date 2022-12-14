using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Models;

namespace liuguang.TlbbGmTool.Services;
public static class ItemService
{
    public static ItemBase? GetItem(int itemBaseId)
    {
        if (SharedData.ItemBaseMap.TryGetValue(itemBaseId, out var itemBaseInfo))
        {
            return itemBaseInfo;
        }
        return null;
    }
    public static string ParseItemName(int itemBaseId)
    {
        if (itemBaseId == 0)
        {
            return string.Empty;
        }
        var itemBaseInfo = GetItem(itemBaseId);
        if (itemBaseInfo != null)
        {
            return $"{itemBaseInfo.Name}(ID: {itemBaseId})";
        }
        return $"未知物品(ID: {itemBaseId})";
    }
}
