# GitHub 同步

此仓库的本地项目是 GitHub 仓库的正常 Git 工作副本。同步通过 Windows Git Credential Manager 中的 `liuxiufan1-cell` 账号认证，凭据不写入项目。仓库级账号选择不会替换电脑上的其他 GitHub 账号。

## 日常使用

在这个仓库目录打开 Unity，保存场景、预制体或脚本后，后台脚本会在约 15 秒无文件变化时提交并推送。连续保存会合并；紧邻上一轮同步的改动可能等待最多约一分钟。Unity 未保存的场景变化不会上传。

本地同步监控进程由 `Tools/Start-GitHubSync.ps1` 启动；Windows 登录启动项可由 `Tools/Enable-GitHubSyncStartup.ps1` 安装。脚本只在电脑运行、账号已登录且能访问 GitHub 时同步。移动仓库文件夹后，需要重新运行 Enable 脚本更新启动路径。

查看状态：

```powershell
Get-Content .local/github-sync.log -Tail 20
git status --short --branch
git log -3 --oneline
```

手工执行单次同步：

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/Sync-GitHub.ps1
```

## 停止或恢复

`Stop-GitHubSync.ps1` 停止当前进程；`Disable-GitHubSyncStartup.ps1` 移除当前用户的登录启动项。重新运行 `Start-GitHubSync.ps1` 即可恢复，重复启动会复用已有进程。

同步仅在 `main` 分支进行。如果检测到远端新提交、合并冲突或正在进行的 Git 操作，会保留文件并记录原因，等待人工处理；不会重写远端历史或自动解决冲突。

## 上传范围

- 提交：`Assets`、`Packages`、`ProjectSettings`、`Tools`、`Docs`、README、Git 属性和忽略规则。
- 排除：Unity 缓存、临时文件、构建输出、日志、IDE 文件、`.local` 和环境文件。
- GitHub 中的历史是保存进度；每次自动提交未必是已通过编译的版本。可运行 `Validate-Unity.ps1` 检查当前状态。

## 验证

同步工具在独立的本地测试仓库验证了：源码提交推送、忽略生成文件和环境文件、无改动不创建提交、连续保存后自动推送、远端前进时停止且不覆盖远端内容。
