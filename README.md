# TlbbGmTool

某网络游戏的单机版本GM工具。

本工具使用C#语言编写，支持 .NET(8.0、6.0)和 .NET Framework(4.8+)运行环境。

- .NET 运行库(新系统推荐) https://dotnet.microsoft.com/zh-cn/download/dotnet

- .NET Framework运行库 https://dotnet.microsoft.com/zh-cn/download/dotnet-framework

> 注意: .NET 8.0 不支持 win10以下的系统，旧版本的系统可以安装 .NET 6.0 或者 .NET Framework 4.8
>

## 编译指南

有两种方式编译此程序，使用visual studio或者使用.NET sdk命令行

### Visual Studio

使用[Visual Studio](https://visualstudio.microsoft.com/zh-hans/vs/)(community版本)打开此项目的sln文件，然后选择编译TlbbGmTool项目即可。

### .NET sdk命令行

安装完.NET sdk之后，打开 CMD 控制台，可以使用`dotnet`工具来编译此项目，根据 sdk 版本执行对应的命令。

```shell
# 编译 .net8.0 版本的
dotnet publish TlbbGmTool -c Release -f net8.0-windows -p:PublishSingleFile=true --no-self-contained
# 编译 .net6.0 版本的
dotnet publish TlbbGmTool -c Release -f net6.0-windows -p:PublishSingleFile=true --no-self-contained
# 编译 .NET Framework 4.8 版本的 (.NET Framework的程序不能打包成单个文件)
dotnet publish TlbbGmTool -c Release -f net48 --no-self-contained
```

编译结果输出大致如下所示，`publish`文件夹下就是构建好的程序：

```
E:\vs_applications\TlbbGmTool> dotnet publish TlbbGmTool -c Release -f net8.0-windows -p:PublishSingleFile=true --no-self-contained
  dbc net8.0-windows 已成功 (0.1) → dbc\bin\Release\net8.0-windows\dbc.dll
  axp net8.0-windows 已成功 (0.2) → axp\bin\Release\net8.0-windows\axp.dll
  TlbbGmTool net8.0-windows 已成功 (0.2) → TlbbGmTool\bin\Release\net8.0-windows\win-x64\publish\

在 0.6 中生成 已成功
```

