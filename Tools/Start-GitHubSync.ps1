$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$stateFolder = Join-Path $projectRoot '.local'
New-Item -ItemType Directory -Path $stateFolder -Force | Out-Null
$pidFile = Join-Path $stateFolder 'sync.pid'
if (Test-Path -LiteralPath $pidFile) {
    $watcherId = [int](Get-Content -LiteralPath $pidFile)
    $existing = Get-CimInstance Win32_Process -Filter "ProcessId = $watcherId" -ErrorAction SilentlyContinue
    if ($existing -and $existing.CommandLine -like '*Sync-GitHub.ps1*' -and $existing.CommandLine.Contains($projectRoot)) {
        Write-Output "GitHub sync is already running (PID $watcherId)."
        exit 0
    }
}
$script = Join-Path $PSScriptRoot 'Sync-GitHub.ps1'
$arguments = @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', ('"' + $script + '"'), '-Watch')
$watcher = Start-Process -FilePath 'powershell.exe' -ArgumentList $arguments -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $stateFolder 'watcher-output.log') -RedirectStandardError (Join-Path $stateFolder 'watcher-error.log')
Write-Output "GitHub sync started (PID $($watcher.Id)). Log: $stateFolder\github-sync.log"
