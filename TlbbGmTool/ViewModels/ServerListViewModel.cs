using System;
using System.Collections.ObjectModel;
using System.Linq;
using liuguang.TlbbGmTool.Common;
using liuguang.TlbbGmTool.Models;
using liuguang.TlbbGmTool.Services;
using liuguang.TlbbGmTool.Views.Server;

namespace liuguang.TlbbGmTool.ViewModels;
public class ServerListViewModel : ViewModelBase
{
    #region Fields
    private ObservableCollection<GameServerViewModel> _serverList = new();
    #endregion

    #region Properties

    public ObservableCollection<GameServerViewModel> ServerList
    {
        get => _serverList;
        set => SetProperty(ref _serverList, value);
    }

    public Command AddServerCommand { get; }

    public Command EditServerCommand { get; }

    public Command DeleteServerCommand { get; }
    #endregion

    public ServerListViewModel()
    {
        AddServerCommand = new(ShowAddDialog);
        EditServerCommand = new(ShowEditDialog, CanEdit);
        DeleteServerCommand = new(ProcessDelete, CanEdit);
    }

    private bool CanEdit(object? parameter)
    {
        if (parameter is GameServerViewModel serverInfo)
        {
            return serverInfo.DbStatus == DbStatus.NotConnect;
        }
        return false;
    }

    private void ShowEditDialog(object? parameter)
    {
        if (parameter is GameServerViewModel serverInfo)
        {
            ShowDialog(new ServerEditorWindow(), (ServerEditorViewModel vm) =>
            {
                vm.ServerInfo = serverInfo;
            });
        }
    }

    private void ShowAddDialog()
    {
        ShowDialog(new ServerEditorWindow(), (ServerEditorViewModel vm) =>
        {
            vm.ServerList = _serverList;
        });
    }

    private async void ProcessDelete(object? parameter)
    {
        if (parameter is not GameServerViewModel serverInfo)
        {
            return;
        }
        //删除确认
        if (!Confirm("删除提示", $"你确定要删除服务器{serverInfo.ServerName}吗?"))
        {
            return;
        }

        ServerList.Remove(serverInfo);
        var serverList = from item in ServerList select item.AsServer();
        try
        {
            await ServerService.SaveGameServersAsync(serverList);
        }
        catch (Exception e)
        {
            ShowErrorMessage("保存配置文件失败", e);
            return;
        }

        ShowMessage("操作成功", "删除服务器成功");
    }
}
