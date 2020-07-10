#Requires -Version 6

Param(
	[string] $name,
	[string] $luisAuthoringRegion,
    [string] $luisAuthoringKey,
	[string] $luisAccountName,
    [string] $luisAccountRegion,
	[string] $luisSubscriptionKey,
    [string] $luisEndpoint,
	[string] $resourceGroup,
    [string] $qnaSubscriptionKey,
    [string] $qnaEndpoint = "https://westus.api.cognitive.microsoft.com/qnamaker/v4.0",
    [switch] $useGov,
	[switch] $useDispatch,
    [string] $languages = "en-us",
    [string] $outFolder = $(Get-Location),
	[string] $logFile = $(Join-Path $PSScriptRoot .. "deploy_cognitive_models_log.txt")
)

. $PSScriptRoot\luis_functions.ps1
. $PSScriptRoot\qna_functions.ps1

# Reset log file
if (Test-Path $logFile) {
	Clear-Content $logFile -Force | Out-Null
}
else {
	New-Item -Path $logFile | Out-Null
}

# Check for AZ CLI and confirm version
if (Get-Command az -ErrorAction SilentlyContinue) {
    $azcliversionoutput = az -v
    [regex]$regex = '(\d{1,3}.\d{1,3}.\d{1,3})'
    [version]$azcliversion = $regex.Match($azcliversionoutput[0]).value
    [version]$minversion = '2.2.0'

    if ($azcliversion -ge $minversion) {
        $azclipassmessage = "AZ CLI passes minimum version. Current version is $azcliversion"
        Write-Debug $azclipassmessage
        $azclipassmessage | Out-File -Append -FilePath $logfile
    }
    else {
        $azcliwarnmessage = "You are using an older version of the AZ CLI, `
    please ensure you are using version $minversion or newer. `
    The most recent version can be found here: http://aka.ms/installazurecliwindows"
        Write-Warning $azcliwarnmessage
        $azcliwarnmessage | Out-File -Append -FilePath $logfile
    }
}
else {
    $azclierrormessage = 'AZ CLI not found. Please install latest version.'
    Write-Error $azclierrormessage
    $azclierrormessage | Out-File -Append -FilePath $logfile
}

# Get mandatory parameters
if (-not $name) {
    $name = Read-Host "? Base name for Cognitive Models"
}

if (-not $luisAuthoringRegion) {
    $luisAuthoringRegion = Read-Host "? LUIS Authoring Region (westus, westeurope, virginia (US gov-only) or australiaeast)"
}

if (-not $luisAuthoringKey) {
	Switch ($luisAuthoringRegion) {
		"westus" { 
			$luisAuthoringKey = Read-Host "? LUIS Authoring Key (found at https://luis.ai/user/settings)"
			Break
		}
        "virginia" {
			$luisAuthoringKey = Read-Host "? LUIS Authoring Key (found at https://luis.azure.us/user/settings)"
			Break
		}
		"westeurope" {
		    $luisAuthoringKey = Read-Host "? LUIS Authoring Key (found at https://eu.luis.ai/user/settings)"
			Break
		}
		"australiaeast" {
			$luisAuthoringKey = Read-Host "? LUIS Authoring Key (found at https://au.luis.ai/user/settings)"
			Break
		}
		default {
			Write-Host "! $($luisAuthoringRegion) is not a valid LUIS authoring region." -ForegroundColor DarkRed
			Break
		}
	}

	if (-not $luisAuthoringKey) {
		Break
	}
}

if (-not $luisAccountName) {
    $luisAccountName = Read-Host "? LUIS Service Name (existing service in Azure required)"
}

if (-not $resourceGroup) {
	$resourceGroup = $name

	$rgExists = az group exists -n $resourceGroup --output json
	if ($rgExists -eq "false")
	{
	    $resourceGroup = Read-Host "? LUIS Service Resource Group (existing service in Azure required)"
	}
}

if (-not $luisSubscriptionKey) {
	$keys = az cognitiveservices account keys list `
        --name $luisAccountName `
        --resource-group $resourceGroup `
        --output json | ConvertFrom-Json

	if ($keys) {
		$luisSubscriptionKey = $keys.key1
	}
	else {
		Write-Host "! Could not retrieve LUIS Subscription Key." -ForgroundColor DarkRed
		Write-Host "+ Verify the -luisAccountName and -resourceGroup parameters are correct." -ForegroundColor Magenta
		Break
	}
}

if (-not $luisAccountRegion) {
	$luisAccountRegion = Read-Host "? LUIS Service Location"
}

if (-not $qnaSubscriptionKey) {	
	$useQna = $false
}
else {
	$useQna = $true
}

if ($useGov){
    $cloud = 'us'
    $gov = $true
}
else {
    $cloud = 'com'
    $gov = $false
}

$azAccount = az account show --output json | ConvertFrom-Json
$azAccessToken = $(Invoke-Expression "az account get-access-token --output json") | ConvertFrom-Json

# Get languages
$languageArr = $languages -split ","

# Initialize settings obj
$settings = @{ defaultLocale = $languageArr[0]; cognitiveModels = New-Object PSObject }

# Deploy localized resources
foreach ($language in $languageArr)
{
    $langCode = $language
    $config = New-Object PSObject

	if ($useDispatch) {
		# Add dispatch to config
		$config | Add-Member `
            -MemberType NoteProperty `
            -Name dispatchModel `
            -Value $(New-Object PSObject)

	    # Initialize Dispatch
        Write-Host "> Initializing $($langCode) dispatch model ..." -NoNewline
		$dispatchName = "$($name)$($langCode)_Dispatch"
		$dataFolder = Join-Path $PSScriptRoot .. Resources Dispatch $langCode
		(dispatch init `
			--name $dispatchName `
			--luisAuthoringKey $luisAuthoringKey `
			--luisAuthoringRegion $luisAuthoringRegion `
            --gov $gov `
			--dataFolder $dataFolder) 2>> $logFile | Out-Null
        Write-Host "Done." -ForegroundColor Green
	}

    # Deploy LUIS apps
    $luisFiles = Get-ChildItem "$(Join-Path $PSScriptRoot .. 'Resources' 'LU' $langCode)" | Where {$_.extension -eq ".lu"}
	if ($luisFiles) {
		$config | Add-Member `
            -MemberType NoteProperty `
            -Name languageModels `
            -Value @()

		foreach ($lu in $luisFiles)
		{
			# Deploy LUIS model
			$luisApp = DeployLUIS `
				-name $name `
				-luFile $lu `
				-endpoint $luisEndpoint `
				-subscriptionKey $luisAuthoringKey `
				-culture $language `
				-log $logFile

			Write-Host "> Setting LUIS subscription key ..." -NoNewline
			if ($luisApp) {
				# Setting subscription key
				$addKeyResult = bf luis:application:assignazureaccount `
						--accountName $luisAccountName `
						--resourceGroup $resourceGroup `
						--armToken $azAccessToken.accessToken `
						--azureSubscriptionId $azAccount.id `
						--appId $luisApp.id `
						--endpoint $luisEndpoint `
						--subscriptionKey $luisAuthoringKey 2>> $logFile

				if ($addKeyResult -ne "Account successfully assigned.") {
					$luisKeySet = $false
					$addKeyResult >> $logFile
					Write-Host "! Could not assign subscription key automatically. Review the log for more information. " -ForegroundColor DarkRed
					Write-Host "! Log: $($logFile)" -ForegroundColor DarkRed
					Write-Host "+ Please assign your subscription key manually in the LUIS portal." -ForegroundColor Magenta
				}
                else {
                    Write-Host "Done." -ForegroundColor Green
                }

				if ($useDispatch) {
					# Add luis app to dispatch
					Write-Host "> Adding $($langCode) $($lu.BaseName) app to dispatch model ..." -NoNewline
					(dispatch add `
						--type "luis" `
						--name $luisApp.name `
						--id $luisApp.id  `
						--region $luisAuthoringRegion `
						--intentName "l_$($lu.BaseName)" `
						--dataFolder $dataFolder `
						--dispatch "$(Join-Path $dataFolder "$($dispatchName).dispatch")") 2>> $logFile | Out-Null
                     Write-Host "Done." -ForegroundColor Green
				}

				# Add to config 
				$config.languageModels += @{
					id = $lu.BaseName
					name = $luisApp.name
					appid = $luisApp.id
                    endpoint = $luisEndpoint
					authoringkey = $luisAuthoringKey
					subscriptionkey = $luisSubscriptionKey
					version = $luisApp.activeVersion
					region = $luisAccountRegion
				}
			}
			else {
				Write-Host "! Could not create LUIS app. Skipping dispatch add." -ForegroundColor Cyan
			}
		}
	}

	if ($useQna) {
        $qnaFiles = Get-ChildItem "$(Join-Path $PSScriptRoot .. 'Resources' 'QnA' $langCode)" -Recurse | Where {$_.extension -eq ".qna"}
		if ($qnaFiles) {		
            $config | Add-Member `
                -MemberType NoteProperty `
                -Name knowledgebases `
                -Value @()

			foreach ($lu in $qnaFiles)
			{
                # Deploy QnA Knowledgebase
				$qnaKb = DeployKB `
                    -name $name `
                    -luFile $lu `
                    -qnaSubscriptionKey $qnaSubscriptionKey `
                    -qnaEndpoint $qnaEndpoint `
                    -language $langCode `
                    -log $logFile
       
				if ($qnaKb) {
					if ($useDispatch) {
						Write-Host "> Adding $($langCode) $($lu.BaseName) kb to dispatch model ..." -NoNewline    
						(dispatch add `
							--type "qna" `
							--name $lu.BaseName `
							--id $qnaKb.kbId  `
							--key $qnaSubscriptionKey `
							--intentName "q_$($lu.BaseName)" `
							--dataFolder $dataFolder `
							--dispatch "$(Join-Path $dataFolder "$($dispatchName).dispatch")") 2>> $logFile | Out-Null
                        Write-Host "Done." -ForegroundColor Green
					}

                    # get qna details
                    $qnaEndpointKeys = bf qnamaker:endpointkeys:list `
                        --endpoint $qnaEndpoint `
                        --subscriptionKey $qnaSubscriptionKey  | ConvertFrom-Json

                    $qnaKbSettings = bf qnamaker:kb:get `
                        --kbId $qnaKb.kbId `
                        --endpoint $qnaEndpoint `
                        --subscriptionKey $qnaSubscriptionKey | ConvertFrom-Json

					# Add to config
					$config.knowledgebases += @{
						id = $lu.BaseName
						name = $lu.BaseName
						kbId = $qnaKb.kbId
						subscriptionKey = $qnaSubscriptionKey
						endpointKey = $qnaEndpointKeys.primaryEndpointKey
						hostname = $qnaKbSettings.hostName
					}
				}
				else {
					Write-Host "! Could not create knowledgebase. Skipping dispatch add." -ForegroundColor Cyan
				}
			}
		}
		else {
			Write-Host "! No knowledgebases found. Skipping." -ForegroundColor Cyan
		}
	}
	else {
		Write-Host "! No QnA Maker Subscription Key provided. Skipping knowledgebases." -ForegroundColor Cyan
	}

	if ($useDispatch) {
		# Create dispatch model
		Write-Host "> Creating $($langCode) dispatch model ..." -NoNewline
		$dispatch = (dispatch create `
			--dispatch "$(Join-Path $dataFolder "$($dispatchName).dispatch")" `
            --gov $gov `
			--dataFolder  $dataFolder `
			--culture $language) 2>> $logFile
        Write-Host "Done." -ForegroundColor Green

		if (-not $dispatch) {
			Write-Host "! Could not create Dispatch app. Review the log for more information." -ForegroundColor DarkRed
			Write-Host "! Log: $($logFile)" -ForegroundColor DarkRed
			Break
		}
		else {
			$dispatchApp  = $dispatch | ConvertFrom-Json

			# Setting subscription key
			Write-Host "> Setting LUIS subscription key ..." -NoNewline
			$addKeyResult = bf luis:application:assignazureaccount `
					--accountName $luisAccountName `
					--resourceGroup $resourceGroup `
					--armToken $azAccessToken.accessToken `
					--azureSubscriptionId $azAccount.id `
					--appId $dispatchApp.appId `
					--endpoint $luisEndpoint `
					--subscriptionKey $luisAuthoringKey 2>> $logFile

			if ($addKeyResult -ne "Account successfully assigned.") {
				$luisKeySet = $false
				$addKeyResult >> $logFile
				Write-Host "! Could not assign subscription key automatically. Review the log for more information. " -ForegroundColor DarkRed
				Write-Host "! Log: $($logFile)" -ForegroundColor DarkRed
				Write-Host "+ Please assign your subscription key manually in the LUIS portal." -ForegroundColor Magenta
			}
            else {
                Write-Host "Done." -ForegroundColor Green
            }

			# Add to config
			$config.dispatchModel = @{
				type = "dispatch"
				name = $dispatchApp.name
				appid = $dispatchApp.appId
				authoringkey = $luisauthoringkey
                authoringRegion = $luisAuthoringRegion
				subscriptionkey = $luisSubscriptionKey
				region = $luisAccountRegion
                version = $dispatchApp.version
			}
		}
	}

    # Add config to cognitivemodels dictionary
    $settings.cognitiveModels | Add-Member `
        -Type NoteProperty `
        -Name $langCode `
        -Value $config `
        -Force
}

# Write out config to file
$settings | ConvertTo-Json -depth 100 | Out-File -Encoding utf8 $(Join-Path $outFolder "cognitivemodels.json" )