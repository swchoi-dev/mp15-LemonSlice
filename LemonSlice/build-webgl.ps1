$ErrorActionPreference = "Stop"

# Unity Editor and project paths
$unityPath = "C:\Program Files\Unity\Hub\Editor\6000.3.9f1\Editor\Unity.exe"
$projectPath = $PSScriptRoot
$logPath = Join-Path $projectPath "unity-build.log"
$buildIndex = Join-Path $projectPath "Build\WebGL\index.html"

if (-not (Test-Path -LiteralPath $unityPath -PathType Leaf)) {
    throw "Unity Editor not found: $unityPath"
}

if (-not (Test-Path -LiteralPath (Join-Path $projectPath "Assets") -PathType Container)) {
    throw "Unity project Assets folder not found: $projectPath"
}

# Remove the previous entry point to avoid treating stale output as success.
if (Test-Path -LiteralPath $buildIndex) {
    Remove-Item -LiteralPath $buildIndex -Force
}

$unityArgs = @(
    '-batchmode'
    '-nographics'
    '-quit'
    '-projectPath', ('"' + $projectPath + '"')
    '-buildTarget', 'WebGL'
    '-executeMethod', 'BuildScript.BuildWebGL'
    '-logFile', ('"' + $logPath + '"')
)

Write-Host "Starting Unity WebGL build..."
Write-Host "Project: $projectPath"
Write-Host "Log: $logPath"

$timer = [System.Diagnostics.Stopwatch]::StartNew()
try {
    $process = Start-Process -FilePath $unityPath -ArgumentList $unityArgs -Wait -PassThru
}
catch {
    Write-Host "Failed to launch Unity: $($_.Exception.Message)"
    throw
}
finally {
    $timer.Stop()
}

$exitCode = $process.ExitCode
Write-Host "Unity exit code: $exitCode"
Write-Host "Elapsed: $($timer.Elapsed.ToString('hh\:mm\:ss'))"

if ($exitCode -ne 0) {
    if (Test-Path -LiteralPath $logPath) {
        Write-Host "===== Unity build log (last 80 lines) ====="
        Get-Content -LiteralPath $logPath -Tail 80
    }
    throw "Unity WebGL build failed (exit code: $exitCode)"
}

if (-not (Test-Path -LiteralPath $buildIndex -PathType Leaf)) {
    if (Test-Path -LiteralPath $logPath) {
        Write-Host "===== Unity build log (last 80 lines) ====="
        Get-Content -LiteralPath $logPath -Tail 80
    }
    throw "Build completed but index.html was not found: $buildIndex. Check BuildScript.BuildWebGL output path."
}

Write-Host "Unity WebGL build completed successfully."
Write-Host "Output: $buildIndex"
