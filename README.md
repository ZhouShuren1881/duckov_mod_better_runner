## C# 版本兼容性

- 游戏使用的 Unity Engine 使用了 `.net standard 2.1`，因而编译目标版本不能低于此项
- 请在 csproj 中设置 `<TargetFramework>netstandard2.1</TargetFramework>`
- 使用 dotnet 8+ 编译

## 核心变更

修改核心思想：

1. 如果用户没有主动变更跑步状态，下次 Update 前恢复 runInputBuffer 的值（两次 Update 之间 runInputBuffer 的值可能被修改）。
2. 监听用户鼠标输入和角色跑步状态，如果角色退出跑步状态时，用户鼠标左键未抬起，开火。
