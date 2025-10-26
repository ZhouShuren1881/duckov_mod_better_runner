## C# 版本兼容性
- 游戏使用的 Unity Engine 使用了 `.net standard 2.1`，因而编译目标版本不能低于此项
- 请在 csproj 中设置 `<TargetFramework>netstandard2.1</TargetFramework>`
- 使用 dotnet 8+ 编译


## TODO
参考如下内容，将 Harmony 合并进 dll 文件中。
```text
四、使用 ILMerge 或 ILRepack（将多个 DLL 合并成一个）
如果你想将多个 DLL 合并成一个 DLL 文件（例如为了简化部署），可以使用：

🔹 ILMerge（仅支持 .NET Framework 项目）
BASH
ILMerge /out:Merged.dll Main.dll Dependency1.dll Dependency2.dll
🔹 ILRepack（支持 .NET Core/.NET 5+）
安装方式（使用 NuGet）：

SH
dotnet tool install --global ilrepack
使用方式：

SH
ilrepack /out:Merged.dll Main.dll Dependency1.dll
```

## 核心变更
修改核心思想：如果用户没有主动变更跑步状态，下次 Update 前恢复 runInputBuffer 的值（两次 Update 之间 runInputBuffer 的值可能被修改）。

原有Update代码。
```csharp
// Code in function Duckov.Core -> InputManager.Update()
if (runInput)
{
  if (runInptutThisFrame)
  {
    runInputBuffer = !runInputBuffer;
  }
}
else if (moveAxisInput.magnitude < 0.1f)
{
  runInputBuffer = false;
}
else if (adsInput)
{
  runInputBuffer = false;
}
characterMainControl.SetRunInput(useRunInputBuffer ? runInputBuffer : runInput);
```
