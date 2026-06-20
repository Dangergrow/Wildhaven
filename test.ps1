# Wildhaven CLI test launcher
# Usage: .\test.ps1
# Automatically finds Unity.exe and runs all tests in batch mode.

$unityPath = "C:\Program Files\Unity\Hub\Editor\6000.5.0f1\Editor\Unity.exe"
if (-not (Test-Path $unityPath)) {
    Write-Host "Searching for Unity..."
    $unityPath = (Get-ChildItem "$env:ProgramFiles\Unity\Hub\Editor" -Recurse -Filter "Unity.exe" -Depth 2 | Select-Object -First 1).FullName
}
if (-not $unityPath) {
    Write-Host "ERROR: Unity.exe not found!"
    exit 1
}

$projectPath = Split-Path $PSScriptRoot -Parent
$logFile = Join-Path $PSScriptRoot "test.log"

Write-Host "Unity: $unityPath"
Write-Host "Project: $projectPath"
Write-Host "Running tests..."

& $unityPath -batchmode -quit -projectPath $projectPath -executeMethod TestRunner.RunAllTests -logFile $logFile

if ($LASTEXITCODE -eq 0) {
    Write-Host "Tests complete. Results in: $logFile"
    Select-String -Path $logFile -Pattern "\[TEST\]" | ForEach-Object { $_.Line }
    $pass = Select-String -Path $logFile -Pattern "\[TEST\].*PASS" | Measure-Object | Select-Object -ExpandProperty Count
    $fail = Select-String -Path $logFile -Pattern "\[TEST\].*FAIL|ERROR" | Measure-Object | Select-Object -ExpandProperty Count
    Write-Host "`nPassed: $pass  Failed: $fail"
} else {
    Write-Host "Unity exited with code $LASTEXITCODE"
    Get-Content $logFile -Tail 20
}
