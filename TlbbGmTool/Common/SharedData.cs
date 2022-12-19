using System.Collections.Generic;
using liuguang.TlbbGmTool.Models;

namespace liuguang.TlbbGmTool.Common;

/// <summary>
/// 全局共享的数据
/// </summary>
public static class SharedData
{
    /// <summary>
    /// 门派名称
    /// </summary>
    /// <returns></returns>
    public static readonly SortedDictionary<int, string> MenpaiMap = new();
    /// <summary>
    /// 攻击属性名称
    /// </summary>
    public static readonly SortedDictionary<int, string> Attr0Map = new();
    /// <summary>
    /// 防御属性名称
    /// </summary>
    public static readonly SortedDictionary<int, string> Attr1Map = new();
    /// <summary>
    /// 物品定义列表
    /// </summary>
    /// <returns></returns>
    public static readonly SortedDictionary<int, ItemBase> ItemBaseMap = new();
    /// <summary>
    /// 心法
    /// </summary>
    /// <returns></returns>
    public static readonly Dictionary<int, XinFaBase> XinFaMap = new();
    /// <summary>
    /// 珍兽技能
    /// </summary>
    /// <returns></returns>
    public static readonly SortedDictionary<int, PetSkillBase> PetSkillMap = new();
    /// <summary>
    /// 暗器状态名称
    /// </summary>
    public static readonly SortedDictionary<int, string> DarkImpactMap = new();

    /// <summary>
    /// 清理txt数据缓存
    /// </summary>
    public static void Clear()
    {
        ItemBaseMap.Clear();
        XinFaMap.Clear();
        PetSkillMap.Clear();
        DarkImpactMap.Clear();
    }
}
