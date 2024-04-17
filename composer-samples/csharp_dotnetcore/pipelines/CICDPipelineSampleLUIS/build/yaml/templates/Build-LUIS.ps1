# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
# 
# Builds and trains LUIS models. Used in conjunction with CICD, performs the following:
#  - Determines what models to build based on the recognizer configured for each dialog and writes a luConfig file with the list
#  - Builds Orchestrator language models (english and multilingual) snapshot files
#  - Creates configuration file used by runtime (orchestrator.settings.json)

Param(
	[string] $outputDirectory,
	[string] $sourceDirectory,
	[string] $crossTrainedLUDirectory,
	[string] $authoringKey,
	[string] $authoringRegion,
	[string] $botName
)

# Import script with common functions
. ($PSScriptRoot + "/LUUtils.ps1")

if ($PSBoundParameters.Keys.Count -lt 5) {
    Write-Host "Builds, trains and publishes LUIS models" 
    Write-Host "Usage:"
    Write-Host "`t Build-LUIS.ps1 -outputDirectory ./generated -sourceDirectory ./ -crossTrainedLUDirectory ./generated/interruption -authoringKey 12345612345 -authoringRegion westus -botName MyBotName"  
    Write-Host "Parameters: "
    Write-Host "`t  outputDirectory - Directory for processed config file"
    Write-Host "`t  sourceDirectory - Directory containing bot's source code."
    Write-Host "`t  crossTrainedLUDirectory - Directory containing .lu/.qna files to process."
    Write-Host "`t  authoringKey - LUIS Authoring key."
    Write-Host "`t  authoringRegion - LUIS Authoring region. [westus|westeurope|australiaeast]"
    Write-Host "`t  botName - Bot's name."
    exit 1
}

# Find the lu models for the dialogs configured to use a LUIS recognizer
$models = Get-LUModels -recognizerType "Microsoft.LuisRecognizer" -crossTrainedLUDirectory $crossTrainedLUDirectory -sourceDirectory $sourceDirectory
if ($models.Count -eq 0)
{
    Write-Host "No LUIS models found."
    exit 0        
}

# Create luConfig file with a list of the LUIS models
$luConfigFile = "$crossTrainedLUDirectory/luConfigLuis.json"
Write-Host "Creating $luConfigFile..."
New-LuConfigFile -luConfig $luConfigFile -luModels $models -path "."

# Output the generated settings
Get-Content $luConfigFile

# Publish and tain LUIS models
bf luis:build --out $outputDirectory --authoringKey $authoringKey --region $authoringRegion --botName $botName --suffix composer --force --log --luConfig $luConfigFile
