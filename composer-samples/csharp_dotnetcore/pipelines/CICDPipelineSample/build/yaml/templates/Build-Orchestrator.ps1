# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
# 
# Builds and trains orchestrator models. Used in conjunction with CICD, performs the following:
#  - Determines what models to build based on the recognizer configured for each dialog
#  - Downloads base model(s) - English, Multilingual depending on configuration
#  - Builds Orchestrator language models (english and multilingual) snapshot files
#  - Creates configuration file used by runtime (orchestrator.settings.json)

Param(
	[string] $outputDirectory,
	[string] $sourceDirectory,
	[string] $crossTrainedLUDirectory,
	[string] $appSettingsFile
)

# Import script with common functions
. ($PSScriptRoot + "/LUUtils.ps1")

if ($PSBoundParameters.Keys.Count -lt 4) {
    Write-Host "Dowloads models and trains orchestrator" 
    Write-Host "Usage:"
    Write-Host "`t Build-Orchestrator.ps1 -outputDirectory ./generated -sourceDirectory ./ -crossTrainedLUDirectory ./generated/interruption -appSettingsFile ./settings/appsettings.json"  
    Write-Host "Parameters: "
    Write-Host "`t  outputDirectory - Directory for processed .blu files"
    Write-Host "`t  sourceDirectory - Directory containing bot's source code."
    Write-Host "`t  crossTrainedLUDirectory - Directory containing .lu/.qna files to process."
    Write-Host "`t  appSettingsFile - Bot appsettings.json file."
    exit 1
}

# Find the lu models for the dialogs configured to use an Orchestrator recognizer
$models = Get-LUModels -recognizerType "Microsoft.OrchestratorRecognizer" -crossTrainedLUDirectory $crossTrainedLUDirectory -sourceDirectory $sourceDirectory
if ($models.Count -eq 0)
{
    Write-Host "No orchestrator models found."
    exit 0        
}
Write-Host "Orchestrator models"
foreach($model in $models)
{
    Write-Host "`t $model"
}

# Load appsettings.json
$appSettings = Get-Content -Path $appSettingsFile | ConvertFrom-Json

# Determine which models we need to download and train.
$useEnglishModel = $false
$useMultilingualModel = $false
Write-Output 'Loading appsettings...'
foreach ($language in $appSettings.languages) {
    if ($language.StartsWith('en')) {
        $useEnglishModel = $true
        Write-Output "`t Found English."
    }
    else {
        $useMultilingualModel = $true
        Write-Output "`t Found multilingual."
    }
}

# Create empty Composer config file for orchestrator settings.
$orchestratorConfig = "{
    orchestrator:{
        models:{},
        snapshots:{}
    }
}" | ConvertFrom-Json


# Download English model and build snapshots
if ($useEnglishModel) 
{
    # Download model and update config
    $modelDirectory = Get-OrchestratoModel -language "english" -modelDirectory "$outputDirectory/orchestratorModels"
    $orchestratorConfig.orchestrator.models | Add-Member -NotePropertyName en -NotePropertyValue (Get-NormalizedPath -path "$modelDirectory")

    # Create luConfig file with a list of English orchestrator models
    $luConfigFile = "$crossTrainedLUDirectory/luConfigOrchestratorEnglish.json"
    Write-Host "Creating $luConfigFile..."
    New-LuConfigFile -luConfig $luConfigFile -luModels ($models | Where-Object {$_ -like '*en*.lu'}) -path "$crossTrainedLUDirectory/"

    # Build snapshots
    bf orchestrator:build --out "$outputDirectory" --model "$modelDirectory" --luconfig $luConfigFile
}

# Download multilanguage model and build snapshots
if ($useMultilingualModel) 
{
    # Download model and update config
    $modelDirectory = Get-OrchestratoModel -language "multilingual" -modelDirectory "$outputDirectory/orchestratorModels"
    $orchestratorConfig.orchestrator.models | Add-Member -NotePropertyName multilang -NotePropertyValue (Get-NormalizedPath -path "$modelDirectory")

    # Create luConfig file with a list of Multilingual orchestrator models
    $luConfigFile = "$crossTrainedLUDirectory/luConfigOrchestratorMultilingual.json"
    Write-Host "Creating $luConfigFile..."
    New-LuConfigFile -luConfig $luConfigFile -luModels ($models | Where-Object {$_ -notlike '*en*.lu'}) -path "$crossTrainedLUDirectory/"

    # Build snapshots
    bf orchestrator:build --out "$outputDirectory" --model "$modelDirectory" --luconfig $luConfigFile
}

# Update and write config file
Write-Output "Overwriting output file $outputDirectory/orchestrator.settings.json"
$bluFiles = Get-ChildItem -Path "$outputDirectory" -Include *.blu -Name
foreach ($bluFile in $bluFiles) 
{
    # Update the key name so composer can recognize (remove the extension, replace . and - by _)
    $key = $bluFile -replace ".{4}$"
    $key = $key.Replace(".", "_")
    $key = $key.Replace("-", "_")
    $orchestratorConfig.orchestrator.snapshots | Add-Member -NotePropertyName "$key" -NotePropertyValue (Get-NormalizedPath -path "$outputDirectory/$bluFile")
}
$orchestratorConfig | ConvertTo-Json | Out-File -FilePath "$outputDirectory/orchestrator.settings.json"

# Output the generated settings
Get-Content "$outputDirectory/orchestrator.settings.json"
