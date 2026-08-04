#Requires -Version 5.1
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$ApiBaseUrl,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[A-Za-z0-9._-]{1,20}$')]
    [string]$ComputerCode,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[A-Za-z0-9._-]{1,100}$')]
    [string]$AgentId,

    [Parameter(Mandatory = $true)]
    [ValidateLength(32, 512)]
    [string]$AgentAccessKey,

    [Parameter(Mandatory = $true)]
    [ValidateLength(32, 512)]
    [string]$PlayerAccessKey,

    [string]$PlayerUser = "$env:USERDOMAIN\$env:USERNAME",
    [string]$PackageRoot = $PSScriptRoot,
    [string]$InstallRoot = "$env:ProgramFiles\ArenaDesk",
    [switch]$EnableSystemCommands,
    [switch]$ValidateOnly
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$serviceName = 'ArenaDeskAgent'
$taskName = 'ArenaDesk Player Screen'
$pipeName = 'ArenaDesk.PlayerScreen'
$agentSource = Join-Path $PackageRoot 'Agent\ArenaDesk.Agent.exe'
$playerSource = Join-Path $PackageRoot 'Player\ArenaDesk.Player.exe'

$apiUri = $null
if (-not [Uri]::TryCreate($ApiBaseUrl, [UriKind]::Absolute, [ref]$apiUri) -or
    $apiUri.Scheme -notin @('http', 'https')) {
    throw 'ApiBaseUrl must be an absolute HTTP or HTTPS URL.'
}

if (-not (Test-Path -LiteralPath $agentSource -PathType Leaf)) {
    throw "Agent executable was not found: $agentSource"
}
if (-not (Test-Path -LiteralPath $playerSource -PathType Leaf)) {
    throw "Player executable was not found: $playerSource"
}
if ([string]::IsNullOrWhiteSpace($PlayerUser) -or $PlayerUser.EndsWith('\')) {
    throw 'PlayerUser must identify the Windows account that runs the Player Screen.'
}

Write-Host "Package validation passed for $ComputerCode ($AgentId)." -ForegroundColor Green
if ($ValidateOnly) {
    return
}

$identity = [Security.Principal.WindowsIdentity]::GetCurrent()
$principal = [Security.Principal.WindowsPrincipal]::new($identity)
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    throw 'Run install.ps1 from an elevated PowerShell window (Run as administrator).'
}

$agentDirectory = Join-Path $InstallRoot 'Agent'
$playerDirectory = Join-Path $InstallRoot 'Player'
$stateDirectory = Join-Path $env:ProgramData 'ArenaDesk'
New-Item -ItemType Directory -Path $agentDirectory, $playerDirectory, $stateDirectory -Force | Out-Null
Copy-Item -Path (Join-Path $PackageRoot 'Agent\*') -Destination $agentDirectory -Recurse -Force
Copy-Item -Path (Join-Path $PackageRoot 'Player\*') -Destination $playerDirectory -Recurse -Force

$hubUrl = "$($ApiBaseUrl.TrimEnd('/'))/hubs/agents"
$commandMode = if ($EnableSystemCommands) { 'System' } else { 'LogOnly' }
$agentSettings = [ordered]@{
    Agent = [ordered]@{
        HubUrl = $hubUrl
        AgentId = $AgentId
        ComputerCode = $ComputerCode
        AccessKey = $AgentAccessKey
        HeartbeatSeconds = 15
        CommandExecutionMode = $commandMode
        StateDirectory = $stateDirectory
        PlayerPipeName = $pipeName
        PlayerAccessKey = $PlayerAccessKey
    }
    Logging = @{ LogLevel = @{ Default = 'Information'; 'Microsoft.AspNetCore.SignalR' = 'Warning' } }
}
$playerSettings = [ordered]@{
    PlayerScreen = [ordered]@{
        PipeName = $pipeName
        AccessKey = $PlayerAccessKey
    }
}
$agentSettings | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $agentDirectory 'appsettings.json') -Encoding UTF8
$playerSettings | ConvertTo-Json -Depth 3 | Set-Content -LiteralPath (Join-Path $playerDirectory 'appsettings.json') -Encoding UTF8

# The service configuration contains the backend and local pipe keys. Standard users cannot read it.
& icacls.exe $agentDirectory /inheritance:r /grant:r '*S-1-5-18:(OI)(CI)F' '*S-1-5-32-544:(OI)(CI)F' | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'Could not secure the Agent directory.' }
& icacls.exe $playerDirectory /inheritance:r /grant:r '*S-1-5-18:(OI)(CI)F' '*S-1-5-32-544:(OI)(CI)F' '*S-1-5-11:(OI)(CI)RX' | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'Could not configure the Player directory permissions.' }

$existingService = Get-Service -Name $serviceName -ErrorAction SilentlyContinue
if ($null -ne $existingService) {
    if ($existingService.Status -ne 'Stopped') {
        Stop-Service -Name $serviceName -Force
    }
    & sc.exe delete $serviceName | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'Could not replace the existing ArenaDesk Agent service.' }
    Start-Sleep -Seconds 2
}

$agentExecutable = Join-Path $agentDirectory 'ArenaDesk.Agent.exe'
New-Service -Name $serviceName -BinaryPathName ('"{0}"' -f $agentExecutable) -DisplayName 'ArenaDesk Agent' -StartupType Automatic | Out-Null
& sc.exe description $serviceName 'ArenaDesk computer control and session service' | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'Could not set the Agent service description.' }
& sc.exe failure $serviceName 'reset= 86400' 'actions= restart/5000/restart/15000/none/0' | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'Could not configure Agent service recovery.' }
& sc.exe failureflag $serviceName 1 | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'Could not enable Agent service recovery.' }

$taskAction = New-ScheduledTaskAction -Execute (Join-Path $playerDirectory 'ArenaDesk.Player.exe') -WorkingDirectory $playerDirectory
$taskTrigger = New-ScheduledTaskTrigger -AtLogOn -User $PlayerUser
$taskPrincipal = New-ScheduledTaskPrincipal -UserId $PlayerUser -LogonType Interactive -RunLevel Limited
$taskSettings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries -RestartCount 3 -RestartInterval (New-TimeSpan -Minutes 1) -StartWhenAvailable
Register-ScheduledTask -TaskName $taskName -Action $taskAction -Trigger $taskTrigger -Principal $taskPrincipal -Settings $taskSettings -Description 'ArenaDesk locked screen and session timer' -Force | Out-Null

Start-Service -Name $serviceName
try {
    Start-ScheduledTask -TaskName $taskName
} catch {
    Write-Warning 'Player Screen will start automatically at the next login.'
}

Write-Host ''
Write-Host 'ArenaDesk installation completed.' -ForegroundColor Green
Write-Host "Service: $serviceName"
Write-Host "Player task: $taskName ($PlayerUser)"
Write-Host "Command mode: $commandMode"
Write-Host 'Run verify.ps1 to check the installation.'
