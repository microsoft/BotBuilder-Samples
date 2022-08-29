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
