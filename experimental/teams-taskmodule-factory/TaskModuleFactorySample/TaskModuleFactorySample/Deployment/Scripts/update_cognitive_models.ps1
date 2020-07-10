#Requires -Version 6

Param(
    [switch] $RemoteToLocal,
    [switch] $useGov,
    [switch] $useLuisGen = $true,
    [string] $configFile = $(Join-Path (Get-Location) 'cognitivemodels.json'),
    [string] $dispatchFolder = $(Join-Path $PSScriptRoot '..' 'Resources' 'Dispatch'),
    [string] $luisFolder = $(Join-Path $PSScriptRoot '..' 'Resources' 'LU'),
	[string] $qnaEndpoint = "https://westus.api.cognitive.microsoft.com/qnamaker/v4.0",
    [string] $qnaFolder = $(Join-Path $PSScriptRoot '..' 'Resources' 'QnA'),
    [string] $lgOutFolder = $(Join-Path (Get-Location) 'Services'),
    [string] $logFile = $(Join-Path $PSScriptRoot .. "update_cognitive_models_log.txt")
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

if ($useGov) {
    $cloud = 'us'
}
else {
    $cloud = 'com'
}

Write-Host "> Getting config file ..." -NoNewline
$languageMap = @{ }
$config = Get-Content -Encoding utf8 -Raw -Path $configFile | ConvertFrom-Json
$config.cognitiveModels.PSObject.Properties | Foreach-Object { $languageMap[$_.Name] = $_.Value }
Write-Host "Done." -ForegroundColor Green

foreach ($langCode in $languageMap.Keys) {
    $models = $languageMap[$langCode]
    $dispatch = $models.dispatchModel

    # try to get dispatch file
    $dispatchFile = $(Join-Path $dispatchFolder $langCode "$($dispatch.name).dispatch")
    if ($dispatch -and (-not $(Test-Path $dispatchFile)))
    {
        # Create a new dispatch file based on configuration
        Write-Host "> Creating new $($langCode) dispatch file ..." -NoNewline
        dispatch init `
            -n $dispatch.name `
            --luisAuthoringKey $dispatch.authoringkey `
            --luisAuthoringRegion $dispatch.authoringRegion `
            --culture $langCode `
            --gov $useGov `
            --dataFolder $(Join-Path $dispatchFolder $langCode) 2>> $logFile | Out-Null
        Write-Host "Done." -ForegroundColor Green
                 
        # Add appId from config
        Write-Host "> Updating $($langCode) dispatch file app id ..." -NoNewline
        $dispatchConfig = Get-Content -Raw -Path $dispatchFile | ConvertFrom-Json
        $dispatchConfig | Add-Member -Name "appId" -value $dispatch.appid -MemberType NoteProperty
        $dispatchConfig | ConvertTo-Json -depth 5 | Set-Content $dispatchFile
        Write-Host "Done." -ForegroundColor Green

        foreach ($luisApp in $models.languageModels)
        {
            # Add the LUIS application to the dispatch model. 
            # If the LUIS application id already exists within the model no action will be taken
            Write-Host "> Adding $($langCode) $($luisApp.id) app to dispatch file ... " -NoNewline
            dispatch add `
                --type "luis" `
                --name $luisApp.name `
                --id $luisApp.appid  `
                --region $luisApp.authoringRegion `
                --intentName "l_$($luisApp.id)" `
                --dispatch $dispatchFile `
                --dataFolder $(Join-Path $dispatchFolder $langCode) --verbose 2>> $logFile | Out-Null
            Write-Host "Done." -ForegroundColor Green
        }

        foreach ($kb in $models.knowledgebases)
        {    
            # Add the knowledge base to the dispatch model. 
            # If the knowledge base id already exists within the model no action will be taken
            if ($dispatch) {
                Write-Host "> Adding $($langCode) $($kb.id) kb to dispatch file ..." -NoNewline
                dispatch add `
                    --type "qna" `
                    --name $kb.name `
                    --id $kb.kbId  `
                    --key $kb.subscriptionKey `
                    --intentName "q_$($kb.id)" `
                    --dispatch $dispatchFile `
                    --dataFolder $(Join-Path $dispatchFolder $langCode) 2>> $logFile | Out-Null
                Write-Host "Done." -ForegroundColor Green
            }
        }
    }
    
    if ($RemoteToLocal) {
        # Update local LU files based on hosted models
        foreach ($luisApp in $models.languageModels) {
             $culture = (bf luis:application:show `
                --appId $luisApp.appId `
                --endpoint $luisApp.endpoint `
                --subscriptionKey $luisApp.authoringkey | ConvertFrom-Json).culture

            $outJson = $(Join-Path $luisFolder $langCode "$($luisApp.id).json")
            $outLU = $(Join-Path $luisFolder $langCode "$($luisApp.id).lu")

            Write-Host "> Updating local $($langCode) $($luisApp.id).lu file ..." -NoNewline
            bf luis:version:export `
                --appId $luisApp.appId `
                --endpoint $luisApp.endpoint `
                --subscriptionKey $luisApp.authoringKey `
                --versionId $luisApp.version `
                --out $outJson `
                --force 2>> $log | Out-Null

            bf luis:convert `
                --in $outJson `
                --out $outLU `
                --force 2>> $logFile | Out-Null
            Write-Host "Done." -ForegroundColor Green

            # Parse LU file
            $id = $luisApp.id
            $outFile = "$($id).json"
            $outFolder = $(Join-Path $luisFolder $langCode)
            $appName = "$($name)$($langCode)_$($id)"

            Write-Host "> Parsing $($langCode) $($luisApp.id) LU file ..." -NoNewline
            bf luis:convert `
                --in $outLU `
                --out $outJson `
                --culture $culture `
                --force 2>> $logFile | Out-Null
            Write-Host "Done." -ForegroundColor Green

            if ($useLuisGen) {
                Write-Host "> Running LuisGen for $($luisApp.id) app ..." -NoNewline
                $luPath = $(Join-Path $luisFolder $langCode "$($luisApp.id).lu")
                RunLuisGen `
                    -luFile $(Get-Item $luPath) `
                    -outName "$($luisApp.id)" `
                    -outFolder $lgOutFolder `
                    -log $logFile
                Write-Host "Done." -ForegroundColor Green
            }

            # Add the LUIS application to the dispatch model. 
            # If the LUIS application id already exists within the model no action will be taken
            if ($dispatch) {
                Write-Host "> Adding $($langCode) $($luisApp.id) app to dispatch model ... " -NoNewline
                (dispatch add `
                        --type "luis" `
                        --name $luisApp.name `
                        --id $luisApp.appid  `
                        --region $luisApp.authoringRegion `
                        --intentName "l_$($luisApp.id)" `
                        --dispatch $(Join-Path $dispatchFolder $langCode "$($dispatch.name).dispatch") `
                        --dataFolder $(Join-Path $dispatchFolder $langCode))  2>> $logFile | Out-Null
                Write-Host "Done." -ForegroundColor Green
            }          
        }

        # Update local LU files based on hosted QnA KBs
        foreach ($kb in $models.knowledgebases) {
            Write-Host "> Updating local $($langCode) $($kb.id).qna file ..." -NoNewline
            bf qnamaker:kb:export `
                --endpoint $qnaEndpoint `
                --environment Prod `
                --kbId $kb.kbId `
                --subscriptionKey $kb.subscriptionKey > $(Join-Path $qnaFolder $langCode "$($kb.id).json")
                
            bf qnamaker:convert `
                --in $(Join-Path $qnaFolder $langCode "$($kb.id).json") `
                --out $(Join-Path $qnaFolder $langCode "$($kb.id).qna") `
                --force 2>> $logFile | Out-Null
            Write-Host "Done." -ForegroundColor Green
		
            # Add the knowledge base to the dispatch model. 
            # If the knowledge base id already exists within the model no action will be taken
            if ($dispatch) {
                Write-Host "> Adding $($langCode) $($kb.id) kb to dispatch model ..." -NoNewline
                (dispatch add `
                    --type "qna" `
                    --name $kb.name `
                    --id $kb.kbId  `
                    --key $kb.subscriptionKey  `
                    --intentName "q_$($kb.id)" `
                    --dispatch $(Join-Path $dispatchFolder $langCode "$($dispatch.name).dispatch") `
                    --dataFolder $(Join-Path $dispatchFolder $langCode))  2>> $logFile | Out-Null
                Write-Host "Done." -ForegroundColor Green
            }
        }
    }
    else {
        # Update each luis model based on local LU files
        foreach ($luisApp in $models.languageModels) {
            $lu = Get-Item -Path $(Join-Path $luisFolder $langCode "$($luisApp.id).lu")
            UpdateLUIS `
                -luFile $lu `
                -appId $luisApp.appid `
                -endpoint $luisApp.endpoint `
                -subscriptionKey $luisApp.authoringKey `
                -culture $langCode `
                -version $luisApp.version `
                -log $logFile

             if ($useLuisGen) {
                Write-Host "> Running LuisGen for $($luisApp.id) app ..." -NoNewline
                $luPath = $(Join-Path $luisFolder $langCode "$($luisApp.id).lu")
                RunLuisGen `
                    -luFile $(Get-Item $luPath) `
                    -outName "$($luisApp.id)" `
                    -outFolder $lgOutFolder `
                    -log $logFile
                Write-Host "Done." -ForegroundColor Green
            }
        }

        # Update each knowledgebase based on local LU files
        foreach ($kb in $models.knowledgebases) {
            $lu = Get-Item -Path $(Join-Path $qnaFolder $langCode "$($kb.id).qna")
            UpdateKB `
                -luFile $lu `
                -kbId $kb.kbId `
                -qnaSubscriptionKey $kb.subscriptionKey `
                -qnaEndpoint $qnaEndpoint `
                -language $langCode `
                -log $logFile
        }
    }

    if ($dispatch) {
        # Update dispatch model
        Write-Host "> Updating $($langCode) dispatch model ..." -NoNewline
        dispatch refresh `
            --gov $useGov `
            --version $dispatch.version `
            --dispatch $(Join-Path $dispatchFolder $langCode "$($dispatch.name).dispatch") `
            --dataFolder $(Join-Path $dispatchFolder $langCode) 2>> $logFile | Out-Null
        Write-Host "Done." -ForegroundColor Green

        if ($useLuisGen) {
            # Update dispatch.cs file
            Write-Host "> Running LuisGen for Dispatch app..." -NoNewline
			bf luis:generate:cs `
                --in $(Join-Path $dispatchFolder $langCode "$($dispatch.name).json") `
                --className "DispatchLuis" `
                --out $lgOutFolder `
                --force 2>> $logFile | Out-Null 
            Write-Host "Done." -ForegroundColor Green
		}
    }
}

Write-Host "> Update complete." -ForegroundColor Green