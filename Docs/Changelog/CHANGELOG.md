# 更新日志 / Changelog

日期使用日本时间（UTC+09:00）。按新到旧记录项目改动，不以文档记录代替实际验证。

## 2026-10-24

### 项目文档

- README 改为中文、日语、英文三份，增加语言切换链接。
- 说明 Unity 版本、本地打开方式、主场景、已实现的操作和目录用途。
- 将更新日志放在 `Docs/Changelog`，将审查记录放在 `Docs/Reviews`。
- 记录现有验证结果、旧问题和未验证范围。本次仅修改文档。

### 保留开发素材，移除辅助代码

关联：[PR #2](https://github.com/liuxiufan1-cell/Game-Unity-2D-Homework/pull/2) · [合并提交 ed937e1](https://github.com/liuxiufan1-cell/Game-Unity-2D-Homework/commit/ed937e1a814cbd791a2e67a88463a1c23f5199c8)

- 恢复 22 张图片、7 个动画片段、2 个动画控制器、9 个素材预制体及其元数据。
- 示例 C# 代码和 Demo 场景继续移除。
- 移除功能验证、预览渲染、场景重建等额外 Editor 脚本。
- 清理 7 个恢复预制体中的 12 个旧示例组件，以及 4 个动画中的 5 个旧示例回调。
- 将 `EnemySample.prefab` 原本缺失的动画控制器引用接到现有 `EnemyAnimator.controller`。
- 将 Goldmetal 的原始版权和使用条件保存在根目录 `THIRD_PARTY_NOTICES.txt`，保留图片素材。
- 现有游戏代码、主场景、游戏预制体与动画、输入配置、依赖和工程设置均未改写。

验证：Unity `6000.3.13f1` 导入与编译通过；14 个预制体和主场景的脚本/组件引用检查通过；53 个图片、动画及控制器资源导入通过；生成、金币计分、死亡重生和终点状态自检通过。旧有 Unity 搜索索引异常仍存在，详见[审查记录](../Reviews/2026-10-24.md)。

### 首轮仓库清理

关联：[PR #1](https://github.com/liuxiufan1-cell/Game-Unity-2D-Homework/pull/1) · [合并提交 9ead843](https://github.com/liuxiufan1-cell/Game-Unity-2D-Homework/commit/9ead8432df82e28ff7082a4c62b58a55b34e9226)

- 删除 186 个当时未使用的示例、模板、Demo、图片及元数据文件，共 6,075,732 字节。
- 所有保留文件内容不变；未发现新增本地 GUID 引用缺失。
- 清理前后导入、编译和基础流程自检均通过。
- 本条记录描述首轮改动。开发素材已在上方的 PR #2 中恢复，不能把首轮删除数量当作当前仓库的最终净删除量。
