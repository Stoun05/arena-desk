#Requires -Version 5.1
[CmdletBinding()]
param(
    [string]$InstallRoot = "$env:ProgramFiles\ArenaDesk",
    [switch]$SkipApiCheck
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$failures = [System.Collections.Generic.List[string]]::new()
$warnings = [System.Collections.Generic.List[string]]::new()
$serviceName = 'ArenaDeskAgent'
$taskName = 'ArenaDesk Player Screen'
$agentSettingsPath = Join-Path $InstallRoot 'Agent\appsettings.json'
$playerSettingsPath = Join-Path $InstallRoot 'Player\appsettings.json'

foreach ($requiredFile in @(
    (Join-Path $InstallRoot 'Agent\ArenaDesk.Agent.exe'),
    (Join-Path $InstallRoot 'Player\ArenaDesk.Player.exe'),
    $agentSettingsPath,
    $playerSettingsPath
)) {
    if (-not (Test-Path -LiteralPath $requiredFile -PathType Leaf)) {
        $failures.Add("Missing file: $requiredFile")
    }
}

$service = Get-Service -Name $serviceName -ErrorAction SilentlyContinue
if ($null -eq $service) {
    $failures.Add("Windows service is not registered: $serviceName")
} elseif ($service.Status -ne 'Running') {
    $failures.Add("Windows service is $($service.Status), not Running.")
} else {
    Write-Host 'OK  Agent service is running.' -ForegroundColor Green
}

$task = Get-ScheduledTask -TaskName $taskName -ErrorAction SilentlyContinue
if ($null -eq $task) {
    $failures.Add("Scheduled task is not registered: $taskName")
} else {
    Write-Host "OK  Player task is $($task.State)." -ForegroundColor Green
}

if ((Test-Path -LiteralPath $agentSettingsPath) -and (Test-Path -LiteralPath $playerSettingsPath)) {
    $agentSettings = Get-Content -LiteralPath $agentSettingsPath -Raw | ConvertFrom-Json
    $playerSettings = Get-Content -LiteralPath $playerSettingsPath -Raw | ConvertFrom-Json
    if ($agentSettings.Agent.PlayerPipeName -ne $playerSettings.PlayerScreen.PipeName) {
        $failures.Add('Agent and Player pipe names do not match.')
    }
    if ($agentSettings.Agent.PlayerAccessKey -ne $playerSettings.PlayerScreen.AccessKey) {
        $failures.Add('Agent and Player access keys do not match.')
    }
    if ([string]::IsNullOrWhiteSpace($agentSettings.Agent.ComputerCode)) {
        $failures.Add('ComputerCode is empty in Agent configuration.')
    } else {
        Write-Host "OK  Computer code: $($agentSettings.Agent.ComputerCode)" -ForegroundColor Green
    }

    if (-not $SkipApiCheck) {
        try {
            $hubUri = [Uri]$agentSettings.Agent.HubUrl
            $apiBaseUrl = $hubUri.GetLeftPart([UriPartial]::Authority)
            $health = Invoke-RestMethod -Uri "$apiBaseUrl/health/ready" -TimeoutSec 10
            Write-Host "OK  API readiness: $health" -ForegroundColor Green
        } catch {
            $warnings.Add("API readiness check failed: $($_.Exception.Message)")
        }
    }
}

$playerProcess = Get-Process -Name 'ArenaDesk.Player' -ErrorAction SilentlyContinue
if ($null -eq $playerProcess) {
    $warnings.Add('Player Screen is not running in the current user session; sign out and sign in once.')
} else {
    Write-Host 'OK  Player Screen process is running.' -ForegroundColor Green
}

foreach ($warning in $warnings) {
    Write-Warning $warning
}
if ($failures.Count -gt 0) {
    foreach ($failure in $failures) {
        Write-Host "FAIL  $failure" -ForegroundColor Red
    }
    exit 1
}

Write-Host 'ArenaDesk installation verification passed.' -ForegroundColor Green
