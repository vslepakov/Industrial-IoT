<#
 .SYNOPSIS
    Installs IoT edge 

 .DESCRIPTION
    Installs IoT edge on either linux or windows vm and enrolls vm in DPS.

 .PARAMETER dpsConnString
    The Dps connection string

 .PARAMETER idScope
    The Dps id scope
#>
param(
    [Parameter(Mandatory)]
    [string] $dpsConnString,
    [Parameter(Mandatory)]
    [string] $idScope
)

$path = Split-Path $script:MyInvocation.MyCommand.Path
Start-Transcript -path (join-path $path "vm-setup.log")

$enrollPath = join-path $path vm-enroll.ps1
if ($PsVersionTable.Platform -eq "Unix") {

    $file = "/etc/iotedge/config.yaml"
    if (Test-Path $file) {
        $backup = "$($file)-backup"
        if (Test-Path $backup) {
            Write-Host "Already configured."
            return
        }
        $configyml = Get-Content $file -Raw
        if ([string]::IsNullOrWhiteSpace($configyml)) {
            throw new "$($file) empty."
        }
        $configyml | Out-File $backup -Force
    }
    else {
        throw new "$($file) does not exist."
    }

    Write-Host "Create new IoT Edge enrollment."
    $enrollment = & $enrollPath -dpsConnString $dpsConnString
    Write-Host "Configure and initialize IoT Edge on Linux using enrollment information."

    # comment out existing 
    $configyml.Replace("`nprovisioning:", "`n#provisioning:")
    $configyml.Replace("`n  source:", "`n#  source:")
    $configyml.Replace("`n  device_connection_string:", "`n#  device_connection_string:")
    $configyml.Replace("`n  dynamic_reprovisioning:", "`n#  dynamic_reprovisioning:")

    # add dps setting
    $configyml += "`n"
    $configyml += "`n ########################################################################"
    $configyml += "`n # DPS symmetric key provisioning configuration - added by vm-setup.ps1 #"
    $configyml += "`n ########################################################################"
    $configyml += "`n"
    $configyml += "`nprovisioning:"
    $configyml += "`n   source: `"dps`""
    $configyml += "`n   global_endpoint: `"https://global.azure-devices-provisioning.net`""
    $configyml += "`n   scope_id: `"$($idScope)`""
    $configyml += "`n   attestation:"
    $configyml += "`n      method: `"symmetric_key`""
    $configyml += "`n      registration_id: `"$($enrollment.registrationId)`""
    $configyml += "`n      symmetric_key: `"$($enrollment.primaryKey)`""
    $configyml += "`n"
    $configyml += "`n ########################################################################"
    $configyml += "`n"

    $configyml | Out-File $file -Force
}
else {

    Write-Host "Create new IoT Edge enrollment."
    $enrollment = & $enrollPath -dpsConnString $dpsConnString

    Write-Host "Configure and initialize IoT Edge on Windows using enrollment information."
    . { Invoke-WebRequest -useb https://aka.ms/iotedge-win } | Invoke-Expression; `
        Install-IoTEdge -Dps -ScopeId $idScope -ContainerOs Windows -RegistrationId `
            $enrollment.registrationId -SymmetricKey $enrollment.primaryKey

    # todo: Test edge
}
