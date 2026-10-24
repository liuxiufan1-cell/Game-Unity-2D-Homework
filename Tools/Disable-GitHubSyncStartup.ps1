$ErrorActionPreference = 'Stop'
$startupFolder = [Environment]::GetFolderPath('Startup')
$shortcutPath = Join-Path $startupFolder 'Game-Unity-2D-Homework GitHub Sync.lnk'
if (Test-Path -LiteralPath $shortcutPath) { Remove-Item -LiteralPath $shortcutPath }
Write-Output 'GitHub sync startup disabled. Use Stop-GitHubSync.ps1 to stop the current watcher.'
