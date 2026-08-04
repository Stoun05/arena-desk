#Requires -Version 5.1
[CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'High')]
param(
    [string]$InstallRoot = "$env:ProgramFiles\ArenaDesk",
    [switch]$RemoveData
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$identity = [Security.Principal.WindowsIdentity]::GetCurrent()
$principal = [Security.Principal.WindowsPrincipal]::new($identity)
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    throw 'Run uninstall.ps1 from an elevated PowerShell window (Run as administrator).'
}

$serviceName = 'ArenaDeskAgent'
$taskName = 'ArenaDesk Player Screen'
$resolvedInstallRoot = [IO.Path]::GetFullPath($InstallRoot).TrimEnd('\')
$programFilesRoot = [IO.Path]::GetFullPath($env:ProgramFiles).TrimEnd('\')
if ($resolvedInstallRoot.Length -le $programFilesRoot.Length -or $resolvedInstallRoot -eq $programFilesRoot) {
    throw 'InstallRoot must be a child directory, not Program Files itself.'
}

if ($PSCmdlet.ShouldProcess('ArenaDesk Windows components', 'Uninstall')) {
    Stop-Process -Name 'ArenaDesk.Player' -Force -ErrorAction SilentlyContinue
    Unregister-ScheduledTask -TaskName $taskName -Confirm:$false -ErrorAction SilentlyContinue

    $service = Get-Service -Name $serviceName -ErrorAction SilentlyContinue
    if ($null -ne $service) {
        if ($service.Status -ne 'Stopped') {
            Stop-Service -Name $serviceName -Force
        }
        & sc.exe delete $serviceName | Out-Null
        if ($LASTEXITCODE -ne 0) { throw 'Could not delete the ArenaDesk Agent service.' }
    }

    if (Test-Path -LiteralPath $resolvedInstallRoot) {
        Remove-Item -LiteralPath $resolvedInstallRoot -Recurse -Force
    }
    if ($RemoveData) {
        $stateDirectory = Join-Path $env:ProgramData 'ArenaDesk'
        if (Test-Path -LiteralPath $stateDirectory) {
            Remove-Item -LiteralPath $stateDirectory -Recurse -Force
        }
    }
}

Write-Host 'ArenaDesk Windows components were removed.' -ForegroundColor Green
