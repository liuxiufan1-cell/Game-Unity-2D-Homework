param(
    [switch]$Watch,
    [int]$QuietSeconds = 15,
    [int]$PollSeconds = 5
)
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
Set-Location -LiteralPath $projectRoot
$env:GIT_TERMINAL_PROMPT = '0'
$stateFolder = Join-Path $projectRoot '.local'
New-Item -ItemType Directory -Path $stateFolder -Force | Out-Null
$logPath = Join-Path $stateFolder 'github-sync.log'

function Write-SyncLog([string]$Message) {
    $line = (Get-Date -Format 'yyyy-MM-dd HH:mm:ss') + ' ' + $Message
    Add-Content -LiteralPath $logPath -Value $line -Encoding UTF8
    Write-Output $line
}
function Invoke-ProjectGit {
    param([Parameter(ValueFromRemainingArguments = $true)][string[]]$GitArguments)
    $savedPreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
        $result = & git -c credential.interactive=false @GitArguments 2>&1
        $gitExitCode = $LASTEXITCODE
    }
    finally { $ErrorActionPreference = $savedPreference }
    if ($gitExitCode -ne 0) { throw ($result -join [Environment]::NewLine) }
    return $result
}
function Get-SourceFingerprint {
    # Git's file list excludes Library, Temp, builds, logs and local state.
    $files = Invoke-ProjectGit -GitArguments @('-c', 'core.quotepath=false', 'ls-files', '--cached', '--others', '--exclude-standard')
    $records = foreach ($file in $files) {
        $path = Join-Path $projectRoot $file
        if (Test-Path -LiteralPath $path -PathType Leaf) {
            $item = Get-Item -LiteralPath $path
            "$file|$($item.Length)|$($item.LastWriteTimeUtc.Ticks)"
        }
        else { "$file|deleted" }
    }
    $sha = [System.Security.Cryptography.SHA256]::Create()
    try { return [Convert]::ToBase64String($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes(($records -join "`n")))) }
    finally { $sha.Dispose() }
}
function Sync-Once {
    $branch = (Invoke-ProjectGit -GitArguments @('branch', '--show-current') | Out-String).Trim()
    if ($branch -ne 'main') { throw "Automatic sync requires main; current branch: $branch" }
    foreach ($name in @('MERGE_HEAD', 'CHERRY_PICK_HEAD', 'REVERT_HEAD', 'rebase-merge', 'rebase-apply', 'index.lock')) {
        $path = (Invoke-ProjectGit -GitArguments @('rev-parse', '--git-path', $name) | Out-String).Trim()
        if (Test-Path -LiteralPath $path) { throw "Git operation in progress: $name" }
    }
    $conflicts = Invoke-ProjectGit -GitArguments @('ls-files', '-u')
    if ($conflicts) { throw 'Unresolved merge conflicts; synchronization paused.' }
    Invoke-ProjectGit -GitArguments @('fetch', '--quiet', 'origin', 'main') | Out-Null
    & git merge-base --is-ancestor origin/main HEAD
    if ($LASTEXITCODE -ne 0) { throw 'Remote main has new commits. Reconcile them manually before synchronization.' }
    $changes = Invoke-ProjectGit -GitArguments @('status', '--porcelain', '--untracked-files=all')
    if ($changes) {
        Invoke-ProjectGit -GitArguments @('add', '--all', '--', 'Assets', 'Packages', 'ProjectSettings', 'Tools', 'Docs', 'README.md', '.gitignore', '.gitattributes') | Out-Null
        & git diff --cached --quiet
        if ($LASTEXITCODE -eq 1) {
            $message = 'Sync project changes ' + (Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
            Invoke-ProjectGit -GitArguments @('commit', '-m', $message) | Out-Null
        }
        elseif ($LASTEXITCODE -ne 0) { throw 'Could not inspect staged changes.' }
    }
    $head = (Invoke-ProjectGit -GitArguments @('rev-parse', 'HEAD') | Out-String).Trim()
    $remote = (Invoke-ProjectGit -GitArguments @('rev-parse', 'origin/main') | Out-String).Trim()
    if ($head -ne $remote) {
        Invoke-ProjectGit -GitArguments @('push', 'origin', 'main') | Out-Null
        Write-SyncLog "Pushed $($head.Substring(0, 12)) to origin/main."
    }
}

$lock = $null
try {
    $lock = [IO.File]::Open((Join-Path $stateFolder 'sync.lock'), 'OpenOrCreate', 'ReadWrite', 'None')
    if (-not $Watch) { Sync-Once; exit 0 }
    Set-Content -LiteralPath (Join-Path $stateFolder 'sync.pid') -Value $PID
    Write-SyncLog "Watcher started (PID $PID); synchronization after $QuietSeconds quiet seconds."
    $fingerprint = Get-SourceFingerprint
    $lastChange = Get-Date
    $pending = $true
    $lastAttempt = [DateTime]::MinValue
    $lastError = ''
    while ($true) {
        $current = Get-SourceFingerprint
        if ($current -ne $fingerprint) {
            $fingerprint = $current
            $lastChange = Get-Date
            $pending = $true
        }
        if ($pending -and ((Get-Date) - $lastChange).TotalSeconds -ge $QuietSeconds -and ((Get-Date) - $lastAttempt).TotalSeconds -ge 60) {
            $lastAttempt = Get-Date
            try {
                Sync-Once
                $pending = $false
                $lastError = ''
            }
            catch {
                $message = $_.Exception.Message
                if ($message -ne $lastError) { Write-SyncLog "Sync pending: $message"; $lastError = $message }
            }
        }
        Start-Sleep -Seconds $PollSeconds
    }
}
finally {
    if ($lock) { $lock.Dispose() }
}
