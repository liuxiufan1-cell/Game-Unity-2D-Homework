# 验证记录

日期：2026-10-24（Asia/Tokyo）。验证对象：从用户压缩包导入并整理后的项目；Unity 6000.3.13f1，Windows 64 位。

**结果：项目编译、资源引用检查、自动玩法检查和 Windows 构建通过；保留一项编辑器搜索索引异常与人工通关限制。**

## 验收证据

| 检查 | 结果 | 证据与范围 |
| --- | --- | --- |
| 首次 Unity 导入与编译 | 通过 | batchmode 正常退出，返回码 0；原课堂单例有 CS0618，后续已修正 |
| 整理后编译与资源检查 | 通过 | Assets 模式返回码 0，COURSEWORK_ASSET_VALIDATION_PASSED；82 个对象、1094 个已赋值引用 |
| GUID 与原始资源保留 | 通过 | 338 个原始文件保留、全部原始 GUID 保留、无重复 GUID 和缺失 .meta；正式场景逐字节保持原样 |
| Play Mode 玩法检查 | 通过 | Smoke 模式返回码 0，COURSEWORK_PLAYMODE_SMOKE_TEST_PASSED |
| Windows 64 位构建 | 通过 | Windows 模式返回码 0，BuildResult.Succeeded，0 错误、0 警告，总构建大小 107,024,289 字节 |
| 独立程序启动 | 通过 | 导出的 Windows 程序以 batchmode/nographics 启动，日志出现玩家和 8 个敌人生成，无游戏异常；检查后结束了独立测试进程 |
| GitHub 同步脚本 | 通过 | 隔离仓库验证源码提交推送、生成文件及环境文件排除、无空提交、连续保存后的自动推送、远端前进时不覆盖 |
| 全程人工操作与视觉 | 未运行 | 未用键盘手工通关或完整目视检查渲染结果 |
| 其他平台与设备 | 未运行 | 未构建 Android、WebGL、macOS 等；未测试全部控制器 |
| 课程评分要求 | 待提供 | 用户要求工程整理完成后再讨论老师需求 |

## 实际运行的玩法检查

场景：`Assets/Game/Scenes/2DGameCoursework.unity`。

1. 等待游戏运行，检查 PlayerSpawner 已生成玩家、8 个敌人出生点全部生成对应敌人（4 行走、4 跳跃），32 个金币、4 个 Sprite Shape、1 个 Tilemap 和初始 0 分。
2. 对金币调用实际触发处理，检查得分增加至 10、金币销毁；调用敌人 Die 和玩家 Kill，检查死亡动画后的对象销毁。
3. 等待重生，检查生成新的玩家实例；调用终点触发处理，检查通关状态及最终 10 分。

启动检查发生在第 369 帧（游戏时间约 1.602 秒），销毁检查发生在第 677 帧（约 2.502 秒）。测试调用了金币及终点的触发处理，没有用真实移动路径验证全部碰撞和关卡可达性。

## 失败基线与修正

- 原测试要求 7 个敌人出生点，而原关卡有 8 个。经原始场景核对，将过时的 7/3 预期更新为 8/4；没有改变关卡内容。
- 旧测试使用编辑器墙钟时间等待。启动时的 Unity 搜索索引异常触发本机 Error Pause，游戏停在第 1 帧、游戏时间 0，导致销毁检查误判或超时。
- 批处理验证现在使用游戏时间和独立超时，临时关闭 Error Pause，并在退出时恢复原值。成功运行后核对原 Console flags 8071 已恢复；错误日志仍完整保留。
- 搜索索引异常的堆栈在 UnityEditor.Search.SearchDatabase/IndexationOnStartup 内。它仍是编辑器侧未解决事项。正式 Windows 构建 0 错误、0 警告，独立程序启动未出现该异常。

## 重跑命令

关闭这个项目的 Unity Editor 后，在仓库根目录执行：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Validate-Unity.ps1 -Mode Assets
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Validate-Unity.ps1 -Mode Smoke
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Validate-Unity.ps1 -Mode Windows
```

## 本地证据与产物

- `Artifacts/baseline-import.log`：首次导入与编译。
- `Artifacts/assets.log`：资源引用检查。
- `Artifacts/smoke.log`：最终成功的 Play Mode 检查，保留编辑器搜索索引异常。
- `Artifacts/smoke-before-test-fix.log`、`smoke-before-timing-fix.log`、`smoke-no-game-frames.log`、`smoke-error-pause.log`：失败基线。
- `Artifacts/windows.log`、`windows-build-summary.txt`：构建结果。
- `Artifacts/Build/Windows/2DGameCoursework.exe`：Windows 游戏入口；需保留同目录下的 UnityPlayer.dll、数据目录等文件。
- `Artifacts/player-smoke.log`：独立程序启动日志。
- `Artifacts/asset-migration.json`、`archive-integrity.json`：移动对照与完整性检查。
- `.local/github-sync.log`：后台同步记录。

缓存、构建、原始日志与同步状态保留在本地，不提交 GitHub。仓库中的本记录提供可审查的结果与重跑命令。
