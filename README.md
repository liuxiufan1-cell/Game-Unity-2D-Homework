# 2D 游戏作业 / Game-Unity-2D-Homework

Unity 2D 横版平台游戏。角色可以移动、二段跳、踩敌人、收集金币，并在死亡后重生；关卡使用四季背景、Tilemap 和 Sprite Shape。

## 打开项目

用 **Unity 6000.3.13f1** 打开仓库根目录，然后打开 `Assets/Game/Scenes/2DGameCoursework.unity`。

- 移动：A / D 或左右方向键。
- 跳跃：空格，最多连续两次。
- 金币每枚 10 分；到达终点后显示通关信息和最终得分。

## 目录约定

| 路径 | 内容 |
| --- | --- |
| `Assets/Game/Scenes` | 当前正式关卡 |
| `Assets/Game/Scripts` | 正式游戏代码，按 Player、Enemies、World、Camera、Scoring 分类 |
| `Assets/Game/Prefabs`、`Animations` | 正式游戏的预制体与动画 |
| `Assets/Game/Editor` | 资源检查、运行检查、Windows 导出、预览和原始示例生成器 |
| `Assets/Input` | Input System 配置及自动生成的 C# 包装类 |
| `Assets/Settings` | URP、2D Renderer、Volume 等渲染配置 |
| `Assets/CourseSamples/TCA2026` | 课堂示例与原始练习素材；正式关卡仍引用其中的地形资源，保留勿删 |
| `Assets/ThirdParty` | 导入的背景和平台游戏素材包，保留原目录及作者说明 |
| `Assets/Templates` | 初始空场景与 Unity 模板说明 |
| `Docs` | 项目背景、审查结果、验证记录和 GitHub 使用说明 |
| `Tools` | 验证与同步脚本 |
| `Artifacts`、`.local` | 本地构建、日志、预览及同步状态，不上传 GitHub |

移动 Unity 资源时务必一起移动同名 `.meta`；优先在 Unity Project 窗口内操作。

## 检查与编译

关闭这个项目的 Unity Editor 后，在仓库目录执行：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Validate-Unity.ps1 -Mode Assets
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Validate-Unity.ps1 -Mode Smoke
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Validate-Unity.ps1 -Mode Windows
```

`Assets` 检查正式场景、预制体及引用；`Smoke` 进入 Play Mode 检查生成、计分、死亡、重生及终点；`Windows` 导出 Windows 64 位游戏。日志保存在 `Artifacts`，可执行文件位于 `Artifacts/Build/Windows/2DGameCoursework.exe`。

Unity 菜单 `Coursework/Build Windows Player` 也可导出当前游戏。`Regenerate Original Example Scene` 会覆盖当前关卡和生成资源，仅用于恢复原始示例；它与当前手工修改后的关卡不同。

## GitHub 同步

仓库：https://github.com/liuxiufan1-cell/Game-Unity-2D-Homework

```powershell
# 单次提交并推送保存的项目改动
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Sync-GitHub.ps1
# 启动 / 停止后台自动同步
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Start-GitHubSync.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Stop-GitHubSync.ps1
```

自动同步等待文件停止变化约 15 秒后提交并推送到 `main`，短时间连续保存会合并；网络失败时每分钟重试。同步只包含已保存文件，不会自动验证每次保存的代码能否编译。出现远端新提交或 Git 冲突时停止推送，并在 `.local/github-sync.log` 中记录原因；解决后会重试，不执行强制推送。

Windows 登录自动启动可用 `Tools/Enable-GitHubSyncStartup.ps1` 开启，用 `Tools/Disable-GitHubSyncStartup.ps1` 关闭。详细说明见 [GitHub 同步](Docs/GitHubSync.md)。

## 素材来源

- Simple 2D Platformer BE2：Goldmetal，原作者说明保留在 `Assets/ThirdParty/Simple 2D Platformer BE2/Readme.asset`。
- Free 2D Cartoon Parallax Background：随作业压缩包导入，保留素材包名称及原文件。
- TCA2026：课堂示例与练习素材。

老师的评分要求尚未提供；当前审查不代表已满足课程提交标准。
