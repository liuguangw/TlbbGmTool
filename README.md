# TlbbGmTool

某网络游戏的单机版本GM工具。

本工具使用C#语言编写，支持.Net(6.0)和.NET Framework(4.8+)两种运行环境，如果不能运行，则需要至少安装一种运行库。

- .NET 运行库(新系统推荐) https://dotnet.microsoft.com/zh-cn/download/dotnet

- .NET Framework运行库(4.8版本支持win7) https://dotnet.microsoft.com/zh-cn/download/dotnet-framework

## 编译指南

有两种方式编译此程序，使用visual studio或者使用.NET sdk命令行

### Visual Studio

使用[Visual Studio](https://visualstudio.microsoft.com/zh-hans/vs/)(community版本)打开此项目的sln文件，然后选择编译TlbbGmTool项目即可。

### .NET sdk命令行

安装完.NET sdk之后，可以使用`dotnet`工具来编译此项目。

```bash
dotnet build TlbbGmTool -c Release
```

编译结果输出大致如下所示，`net48`文件夹下的是.NET Framework版本的，`net6.0-windows`文件夹下的是.NET版本的：

```
E:\vs_applications\TlbbGmTool>dotnet build TlbbGmTool -c Release
适用于 .NET MSBuild 版本 17.8.3+195e7f5a3
  正在确定要还原的项目…
  所有项目均是最新的，无法还原。
  dbc -> E:\vs_applications\TlbbGmTool\dbc\bin\Release\net6.0-windows\dbc.dll
  axp -> E:\vs_applications\TlbbGmTool\axp\bin\Release\net6.0-windows\axp.dll
  dbc -> E:\vs_applications\TlbbGmTool\dbc\bin\Release\net48\dbc.dll
  axp -> E:\vs_applications\TlbbGmTool\axp\bin\Release\net48\axp.dll
  TlbbGmTool -> E:\vs_applications\TlbbGmTool\TlbbGmTool\bin\Release\net6.0-windows\TlbbGmTool.dll
  TlbbGmTool -> E:\vs_applications\TlbbGmTool\TlbbGmTool\bin\Release\net48\TlbbGmTool.exe

已成功生成。
    0 个警告
    0 个错误

已用时间 00:00:01.11

E:\vs_applications\TlbbGmTool>
```

