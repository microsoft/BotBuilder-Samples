#
# This extracts contents from or recompresses them back into NuGet .nupkg files.
# Run this to extract before, then recompress after signing assemblies in the packages.
#
param
( 
    [string]$path,
    [switch]$extract,
    [switch]$compress
)
pushd $path

# Download temporary version of Archive module that fixes issue on macOS/Linux with path separator
#Invoke-WebRequest -Uri "https://raw.githubusercontent.com/PowerShell/Microsoft.PowerShell.Archive/master/Microsoft.PowerShell.Archive/Microsoft.PowerShell.Archive.psm1" -OutFile .\archive.psm1
#Import-Module .\archive.psm1

# Ensure Powershell.Archive minimum version 1.2.3.0 is installed. That fixes a path separator issue on macOS/Linux. 
$ver = (Get-Command -Module Microsoft.PowerShell.Archive | Select-Object -Property version -First 1).Version.ToString()
if ($ver -lt '1.2.3.0') { 
    Write-Host "Installing Microsoft.Powershell.Archive 1.2.3.0 (fix for Linux path separator bug)"
    Install-Module -Name Microsoft.PowerShell.Archive -MinimumVersion '1.2.3.0' -AllowClobber -Force -AcceptLicense 
} else { 
    Write-Host "Already installed: Microsoft.Powershell.Archive $ver"
}

[int]$itemsProcessed = 0
if ($extract) {
    # Extract .nupkg packages in the path.
    Get-ChildItem . -Filter *.nupkg | 
    Foreach-Object {
        Write-Host $_.Name
        Rename-Item -Path $_.Name -NewName ($_.BaseName + ".zip")
        Expand-Archive ($_.BaseName + '.zip') -DestinationPath ($_.BaseName)
        Remove-Item -Path ($_.DirectoryName + '\' + $_.BaseName + '.zip')
        $itemsProcessed++
    }
} elseif ($compress) {
    # Compress folders in the path. Name them *.nupkg.
    Get-ChildItem | ?{ $_.PSIsContainer } |
    Foreach-Object {
        Write-Host $_.Name
        Compress-Archive ($_.Name + '\**') -DestinationPath ($_.Name + '.zip')
        Rename-Item -Path ($_.Name + '.zip') -NewName ($_.BaseName + ".nupkg")
        Remove-Item -Path ($_.FullName) -Recurse
        $itemsProcessed++
    }
} else {
    throw 'Error: Missing argument "-Extract" or "-Compress".'
}
if ($itemsProcessed -eq 0) {
    Write-Host "No items found to process in path '$path'."
}

popd