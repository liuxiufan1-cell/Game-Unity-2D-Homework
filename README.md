# Game-Unity-2D-Homework

**简体中文** | [日本語](README.ja.md) | [English](README.en.md)

使用 Unity 制作的 2D 平台跳跃作业。仓库保留可运行的游戏、可继续使用的开发素材，以及本地打开和编辑所需的 Unity 工程文件。

## 开发环境

| 项目 | 版本或路径 |
|---|---|
| Unity Editor | `6000.3.13f1` |
| 渲染管线 | URP `17.3.0`，2D Renderer |
| 输入系统 | Input System `1.19.0` |
| 主场景 | [2DGameCoursework.unity](Assets/Game/Scenes/2DGameCoursework.unity) |

## 在本地打开

1. 克隆仓库，或在 GitHub 选择 **Code → Download ZIP** 后解压。

   ```sh
   git clone https://github.com/liuxiufan1-cell/Game-Unity-2D-Homework.git
   ```

2. 在 Unity Hub 安装并激活 Unity `6000.3.13f1`。
3. 通过 **Add project from disk / 从磁盘添加项目** 添加仓库根目录，即同时包含 `Assets`、`Packages` 和 `ProjectSettings` 的文件夹。
4. 等待首次包解析、资源导入和脚本编译完成。
5. 打开 `Assets/Game/Scenes/2DGameCoursework.unity`，点击 **Play**，并让 Game 窗口获得焦点。

需要导出游戏时，使用 Unity 的 **File → Build Profiles**，安装目标平台模块，并确认主场景位于构建场景列表中。

## 游戏内容与操作

- 左右移动、二段跳、镜头跟随。
- 行走型与跳跃型敌人、踩踏判定、死亡后重新生成。
- 金币收集与计分、终点判定和最终得分显示。

| 操作 | 键盘 | 手柄绑定 |
|---|---|---|
| 左右移动 | `A / D` 或 `← / →` | 左摇杆 |
| 跳跃 / 二段跳 | `Space` | 下方按键，例如 Xbox A |

手柄项来自输入配置，尚未做实体手柄测试。当前玩家脚本使用 `Move` 和 `Jump`；输入资源中的其他默认 Action 不代表游戏已实现对应功能。金币收集时会向 Console 输出得分，通关时显示最终得分。

## 工程结构

| 路径 | 用途 |
|---|---|
| `Assets/Game` | 游戏脚本、主场景、游戏预制体与动画 |
| `Assets/Input` | Input Actions 配置及生成的 C# 包装类 |
| `Assets/CourseSamples` | 保留的地形、动画和素材预制体；旧示例代码已移除 |
| `Assets/ThirdParty` | 背景、角色、敌人、金币等第三方图片素材 |
| `Assets/Settings` | URP、2D Renderer 和渲染配置 |
| `Packages`、`ProjectSettings` | Unity 依赖与工程设置 |
| `Docs/Changelog` | 更新日志 |
| `Docs/Reviews` | 审查记录与验证范围 |

开发素材保留在工程中。恢复的素材预制体已移除依赖旧示例代码的组件和动画回调，使用时需要按实际需求挂接游戏脚本；它们不都具备独立运行的完整角色或管理器行为。

移动或重命名资源时，优先使用 Unity 的 Project 窗口，并保留对应 `.meta`，避免 GUID 引用失效。`Library`、`Temp` 等生成目录不需要提交。

## 验证与记录

最近一次游戏验证基于 `ed937e1`：Unity 导入与编译、14 个预制体及主场景的脚本/引用检查、基础游戏流程自检已通过。验证在 Linux 无图形模式下进行；Windows 编辑器界面、实际画面、实体输入设备及导出的游戏尚未实测。云端日志仍存在旧版本已有的 Unity 搜索索引异常，详见审查记录。

- [更新日志](Docs/Changelog/CHANGELOG.md)
- [2026-10-24 审查记录](Docs/Reviews/2026-10-24.md)
- [第三方素材版权与使用说明](THIRD_PARTY_NOTICES.txt)
