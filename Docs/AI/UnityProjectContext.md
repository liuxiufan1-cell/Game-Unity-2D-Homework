# Unity Project Context

<!-- unity-onboarding:generated:start -->

分析日期：2026-10-24（Asia/Tokyo）。基础远端提交：`a3193df`；游戏来源为用户提供的 `1017_2Dgame_26j70008_リュウシュウファン.zip`，SHA-256：`e0a2c20d18f64c44514c5511ceb92ad265cccd00a4099781e067b9866050abcc`。

## 环境与入口（已确认）

- Unity `6000.3.13f1`，依据 `ProjectSettings/ProjectVersion.txt`。
- URP `17.3.0`，使用 `Assets/Settings/UniversalRP.asset` 和 `Renderer2D.asset`；依据 manifest、GraphicsSettings 及资源 GUID。
- Input System `1.19.0`；Player Settings 输入模式 Both（2），正式角色使用生成的 `InputSystem_Actions`。不要直接编辑生成代码。
- 唯一启用的构建场景：`Assets/Game/Scenes/2DGameCoursework.unity`，依据 `ProjectSettings/EditorBuildSettings.asset`。
- 当前验证平台：Windows 64 位。未推断其他平台已经验证。
- Test Framework `1.6.0`，无项目自定义 asmdef、asmref 或 NUnit 测试程序集。已有自定义 Play Mode 验证器。
- 无已确认的多人联机实现。`com.unity.multiplayer.center` 的存在不代表游戏已联机。

## 结构与架构（已确认）

- `Assets/Game`：正式游戏；MonoBehaviour + 场景组合与 SerializeField 引用。
- `Scripts/Player`：角色输入、物理、死亡；PlayerSpawner Instantiate 玩家，并在死亡事件后重生。
- `Scripts/Enemies`：EnemyBase 和行走、跳跃子类；EnemySpawner 在 Start 生成敌人。
- `Scripts/World`：金币触发器、死亡区域、终点；`Scripts/Scoring` 的 ScoreCounter 是场景生命周期的单例。
- `Scripts/Camera`：相机平滑跟随并限制边界。
- `Assets/Input`：输入资产和自动生成包装类，课堂示例也引用该类。
- `Assets/CourseSamples/TCA2026`：课堂示例；其中的地形预制体、TilePalette、Sprite Shape 仍被正式游戏使用，不能直接删除。
- `Assets/ThirdParty`：两个导入素材包；`Assets/Templates`：初始示例和说明；`Assets/Settings`：渲染配置。
- 无 asmdef：运行时代码由 Assembly-CSharp 编译，Editor 文件夹由 Assembly-CSharp-Editor 编译。
- 正式代码无 namespace，私有序列化字段使用 `[SerializeField] private` 和 camelCase；没有引入新的依赖或命名空间改造。

## 工具与验证

- 本机 Unity Editor 命令行可用；没有发现 Unity MCP 依赖或可用 Unity MCP 工具。未安装桥接包。
- `Tools/Validate-Unity.ps1`：Compile、Assets、Smoke、Windows、Previews。
- `CourseworkBuildTools`：正式场景与预制体引用检查、Windows BuildPipeline 导出。
- `CourseworkPlayModeValidator`：自动进入 Play Mode，检查角色与敌人生成、收集金币、死亡销毁、重生及终点；通过代码触发金币及终点，不能替代实际键盘通关测试。
- `CourseworkSceneBuilder`：原始示例生成器，会重新生成资源和场景；当前关卡有手工修改，不可为了检查而重新生成。
- `Tools/Sync-GitHub.ps1`：文件稳定后提交并推送，冲突与远端前进时等待人工处理；本地日志与构建均被忽略。

## 约束与未知项

- 保留全部原始资源及 `.meta` GUID。移动资产后更新工具中的路径字符串和 Build Settings。
- 原压缩包包含 4 个行走敌人出生点和 4 个跳跃敌人出生点；旧验证脚本的 7 个出生点预期已过时。原始生成器仍生成 7 个，用途与当前正式关卡分开。
- 老师要求、最终提交格式和评分标准尚未知；先完成工程整理和验证，再讨论作业要求。
- 未验证全部输入设备、手工完整通关、性能预算、其他平台和课程合规性。最终证据见 `Docs/Validation.md`。

代表性检查文件：ProjectVersion、manifest、packages-lock、GraphicsSettings、QualitySettings、EditorBuildSettings、ProjectSettings、全部正式运行脚本、Player 与敌人预制体、主场景、正式 Editor 工具、课堂 GameManager/SingletonMonoBehaviour、第三方 Readme。

<!-- unity-onboarding:generated:end -->
