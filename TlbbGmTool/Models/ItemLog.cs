namespace liuguang.TlbbGmTool.Models;

/// <summary>
/// 一条物品记录
/// </summary>
public class ItemLog
{
    /// <summary>
    /// 记录id
    /// </summary>
    public int Id;
    /// <summary>
    /// 所属角色id
    /// </summary>
    public int CharGuid;
    /// <summary>
    /// 物品guid
    /// </summary>
    public int Guid;
    /// <summary>
    /// 世界编号
    /// </summary>
    public int World = 101;
    /// <summary>
    /// 服务器编号
    /// </summary>
    public int Server = 0;
    /// <summary>
    /// 物品编号
    /// </summary>
    public int ItemBaseId;
    /// <summary>
    /// 位置
    /// </summary>
    public int Pos;
    /// <summary>
    /// P1 - P17
    /// </summary>
    public byte[] PData = new byte[17 * 4];
    /// <summary>
    /// 制作者
    /// </summary>
    public string Creator = string.Empty;
    public bool IsValid = true;
    public int DbVersion = 0;
    public string FixAttr = string.Empty;
    public string TVar = string.Empty;
    public int VisualId = 0;
    public int MaxgemId = -1;
}