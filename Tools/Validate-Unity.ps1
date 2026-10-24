param(
    [ValidateSet('Compile', 'Assets', 'Smoke', 'Windows', 'Previews')]
    [string]$Mode = 'Assets',
    [string]$UnityPath
)
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
if (-not $UnityPath) {
    $version = (Get-Content (Join-Path $projectRoot 'ProjectSettings/ProjectVersion.txt') | Select-String '^m_EditorVersion: ').ToString().Split(' ')[1]
    $UnityPath = Join-Path ${env:ProgramFiles} "Unity/Hub/Editor/$version/Editor/Unity.exe"
}
if (-not (Test-Path -LiteralPath $UnityPath)) { throw "Unity Editor not found: $UnityPath" }
New-Item -ItemType Directory -Path (Join-Path $projectRoot 'Artifacts') -Force | Out-Null
$logPath = Join-Path $projectRoot ('Artifacts/' + $Mode.ToLowerInvariant() + '.log')
$unityArguments = @('-batchmode', '-projectPath', ('"' + $projectRoot + '"'), '-logFile', ('"' + $logPath + '"'))
if ($Mode -notin @('Previews', 'Smoke')) { $unityArguments += '-nographics' }
if ($Mode -ne 'Smoke') { $unityArguments += '-quit' }
$method = switch ($Mode) {
    'Assets' { 'CourseworkBuildTools.ValidateAssets' }
    'Smoke' { 'CourseworkPlayModeValidator.Run' }
    'Windows' { 'CourseworkBuildTools.BuildWindows' }
    'Previews' { 'CourseworkPreviewRenderer.RenderSeasonPreviews' }
}
if ($method) { $unityArguments += @('-executeMethod', $method) }
$process = Start-Process -FilePath $UnityPath -ArgumentList $unityArguments -WindowStyle Hidden -PassThru
if (-not $process.WaitForExit(900000)) {
    # Do not kill an Editor that may still be importing or building.
    throw "Validation timed out. Inspect PID $($process.Id) and $logPath before retrying."
}
if ($process.ExitCode -ne 0) { throw "Unity exited with code $($process.ExitCode). See $logPath" }
$marker = switch ($Mode) {
    'Assets' { 'COURSEWORK_ASSET_VALIDATION_PASSED' }
    'Smoke' { 'COURSEWORK_PLAYMODE_SMOKE_TEST_PASSED' }
    'Windows' { 'COURSEWORK_WINDOWS_BUILD_PASSED' }
    'Previews' { 'COURSEWORK_PREVIEWS_RENDERED' }
    'Compile' { 'Exiting batchmode successfully' }
}
if (-not (Select-String -LiteralPath $logPath -SimpleMatch $marker -Quiet)) { throw "Success marker missing. See $logPath" }
Write-Output "$Mode passed. Log: $logPath"
