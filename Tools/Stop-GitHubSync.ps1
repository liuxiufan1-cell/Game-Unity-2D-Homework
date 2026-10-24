$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$pidFile = Join-Path $projectRoot '.local/sync.pid'
if (Test-Path -LiteralPath $pidFile) {
    $watcherId = [int](Get-Content -LiteralPath $pidFile)
    $watcher = Get-CimInstance Win32_Process -Filter "ProcessId = $watcherId" -ErrorAction SilentlyContinue
    if ($watcher -and $watcher.CommandLine -like '*Sync-GitHub.ps1*' -and $watcher.CommandLine.Contains($projectRoot)) {
        Stop-Process -Id $watcherId
        Write-Output 'GitHub sync stopped.'
    }
    else { Write-Output 'GitHub sync is not running.' }
}
