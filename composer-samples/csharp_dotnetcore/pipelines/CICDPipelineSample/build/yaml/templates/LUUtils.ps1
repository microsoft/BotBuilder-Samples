# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
# 
# Contains language understanding functions used in other scripts.
#

# Given directory with lu files, crawls the bot's sourceDirectory to find the recognizers for them 
# and returns a new list of luModels that match the given recognizer kind.
function Get-LUModels
{
    param 
    (
        [string] $recognizerType,
        [string] $crossTrainedLUDirectory,
        [string] $sourceDirectory
    )

    # Get a list of the cross trained lu models to process
    $crossTrainedLUModels = Get-ChildItem -Path $crossTrainedLUDirectory -Filter "*.lu" -file -name

    # Get a list of all the dialog recognizers (exclude bin and obj just in case)
    $luRecognizerDialogs = Get-ChildItem -Path $sourceDirectory -Filter "*??-??.lu.dialog" -file -name -Recurse | Where-Object { $_ -notmatch '^bin*|^obj*' }

    # Create a list of the models that match the given recognizer
    $luModels = @()
    foreach ($luModel in $crossTrainedLUModels) {
        # Load the dialog JSON and find the recognizer kind
        $luDialog = $luRecognizerDialogs | Where-Object { $_ -match "/$luModel.dialog" }
        $dialog = Get-Content -Path "$sourceDirectory/$luDialog" | ConvertFrom-Json
        $recognizerKind = ($dialog | Select -ExpandProperty "`$kind")

        # Add it to the list if it is the expected type
        if ( $recognizerKind -eq $recognizerType)
        {
            $luModels += "$luModel"
        }
    }

    # return the models found
    return $luModels
}

# Creates luConfigFile for a list of lu models
function New-LuConfigFile
{
    param
    (
        [string] $luConfig,
        [string[]] $luModels,
        [string[]] $path
    )

    $luConfigLuis = "{
        models:[]
    }" | ConvertFrom-Json
    
    foreach($model in $models)
    {
        $luConfigLuis.models += "$path/$model"
    }
    
    $luConfigLuis | ConvertTo-Json | Out-File -FilePath $luConfigFile
}

# Downloads the orchestrator models for a given language
function Get-OrchestratoModel
{
    param 
    (
        [string] $language,
        [string] $modelDirectory
    )
    
    # Clean and recreate the model directory
    $outDirectory = "$modelDirectory/$language"
    if ((Test-Path -Path "$outDirectory") -eq $true) 
    {
        Remove-Item -Path "$outDirectory" -Force -Recurse | Out-Null
    }
    Write-Host "Creating $outDirectory folder..."
    New-Item -Path "$outDirectory" -ItemType "directory" -Force | Out-Null
    Write-Host "done."

    # We only support english and multilingual for now
    if ($language -eq "english")
    {
        bf orchestrator:basemodel:get -o "$outDirectory" | Out-Null
    }
    else
    {
        bf orchestrator:basemodel:get -o "$outDirectory" --versionId pretrained.20210205.microsoft.dte.00.06.unicoder_multilingual.onnx | Out-Null
    }

    return $outDirectory
}

# Helper to replace \ by / so paths work on linux and windows
function Get-NormalizedPath
{
    param 
    (
        [string] $path
    )
    return $path.Replace("\", "/")
}
