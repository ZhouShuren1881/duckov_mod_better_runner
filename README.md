# 自动恢复跑步模式 （Better Runner）

[📄 English README.md](./README.en.md)

增强“Shift切换奔跑”模式，跑步状态被开火/瞄准/饮食等操作打断后可以自动恢复，跑动过程中可以开火（为平衡性考虑，开火时为行走状态，结束后恢复）。

- “按住Shift奔跑”保持游戏原有设定。
- 小拇指保护协会呼吁您关注小拇指的健康！

## 🔗 Steam 创意工坊链接
点这里 → [自动恢复跑步模式(Better Runner)](https://steamcommunity.com/sharedfiles/filedetails/?id=3594173614)

## 📥 下载
Mod 压缩包点击 [此处](https://github.com/ZhouShuren1881/duckov_mod_better_runner/releases)。

## 核心思想

1. 如果用户没有主动变更跑步状态，下次 Update 前恢复 runInputBuffer 的值（两次 Update 之间 runInputBuffer 的值可能被修改）
2. 监听用户鼠标输入和角色跑步状态。如果角色退出跑步状态时，用户鼠标左键未抬起，开火

## C# 版本兼容性

- 编译目标 `.NET Standard 2.1`
- 使用 dotnet 8+ 编译 [下载 .NET - Microsoft](https://dotnet.microsoft.com/zh-cn/download)

## 编译流程
1. 修改 csproj 中的 `DuckovPath` 变量为实际游戏目录
2. 从 [Repo Release](https://github.com/ZhouShuren1881/duckov_mod_better_runner/releases) 复制 0Harmony.dll 到项目根目录
3. 执行编译指令 `dotnet build /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary`
