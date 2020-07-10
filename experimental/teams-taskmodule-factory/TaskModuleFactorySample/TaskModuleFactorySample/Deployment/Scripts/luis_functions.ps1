function DeployLUIS ($name, $luFile, $endpoint, $subscriptionKey, $culture, $log)
{
    $id = $luFile.BaseName
    $outFile = Join-Path $luFile.DirectoryName "$($id).json"
    $appName = "$($name)$($culture)_$($id)"
    
    Write-Host "> Converting $($language) $($id) LU file ..." -NoNewline
    bf luis:convert `
        --in $luFile `
        --out $outFile `
        --name $appName `
        --culture $culture `
        --force
    Write-Host "Done." -ForegroundColor Green

    Write-Host "> Deploying $($language) $($id) LUIS app ..." -NoNewline
    $result = bf luis:application:import `
        --in $outFile `
        --name $appName `
        --endpoint $endpoint `
        --subscriptionKey $subscriptionKey `
        --json `
        --save | ConvertFrom-Json

    if ($result.Status -eq "Success") {
        Write-Host "Done." -ForegroundColor Green

        $luisApp = bf luis:application:show `
            --appId $result.id `
            --endpoint $endpoint `
            --subscriptionKey $subscriptionKey | ConvertFrom-Json

        Write-Host "> Training and publishing LUIS app ..." -NoNewline
        $(bf luis:train:run `
            --appId $luisApp.id `
            --endpoint $endpoint `
            --subscriptionKey $subscriptionKey `
            --versionId $luisApp.activeVersion `
            --wait
        & bf luis:application:publish `
            --appId $luisApp.id `
            --endpoint $endpoint `
            --subscriptionKey $subscriptionKey `
            --versionId $luisApp.activeVersion) 2>> $log | Out-Null
        Write-Host "Done." -ForegroundColor Green

        Return $luisApp
    }
    else {
        Write-Host "! Could not deploy LUIS model. Review the log for more information." -ForegroundColor DarkRed
        Write-Host "! Log: $($log)" -ForegroundColor DarkRed
        Return $null
    }
}

function UpdateLUIS ($luFile, $appId, $endpoint, $subscriptionKey, $culture, $version, $log)
{
   $id = $luFile.BaseName
    $outFile = Join-Path $luFile.DirectoryName "$($id).json"
    $appName = "$($name)$($culture)_$($id)"

    Write-Host "> Getting hosted $($culture) $($id) LUIS model settings..." -NoNewline
    $luisApp = bf luis:application:show `
        --appId $appId `
        --endpoint $endpoint `
        --subscriptionKey $subscriptionKey | ConvertFrom-Json
    Write-Host "Done." -ForegroundColor Green


    Write-Host "> Converting $($culture) $($id) LU file ..." -NoNewline
    bf luis:convert `
        --in $luFile `
        --out $outFile `
        --name $appName `
        --culture $culture `
        --force 2>> $log | Out-Null

    if (-not (Test-Path $outFile)) {
        Write-Host "Error." -ForegroundColor Red
        Write-Host "! File not created. Review the log for more information." -ForegroundColor Red
	    Write-Host "! Log: $($log)" -ForegroundColor Red
        Break
    }
    else {
        Write-Host "Done." -ForegroundColor Green
    }


    Write-Host "> Getting current versions ..." -NoNewline
    $versions = bf luis:version:list `
        --appId $appId `
        --endpoint $endpoint `
        --subscriptionKey $subscriptionKey 2>> $log | ConvertFrom-Json
    Write-Host "Done." -ForegroundColor Green


    if ($versions | Where { $_.version -eq $version })
    {
        if ($versions | Where { $_.version -eq "backup" })
        {
            Write-Host "> Deleting old backup version ..." -NoNewline
            bf luis:version:delete `
                --versionId "backup" `
                --appId $appId `
                --endpoint $endpoint `
                --subscriptionKey $subscriptionKey 2>> $log | Out-Null
            Write-Host "Done." -ForegroundColor Green
        }
        
        Write-Host "> Saving current version as backup ..." -NoNewline
        bf luis:version:rename `
            --versionId $version `
            --newVersionId "backup" `
            --appId $appId `
            --endpoint $endpoint `
            --subscriptionKey $subscriptionKey 2>> $log | Out-Null
        Write-Host "Done." -ForegroundColor Green
    }

    Write-Host "> Importing new version ..." -NoNewline
    $result = bf luis:version:import `
        --in $outFile `
        --appId $appId `
        --endpoint $endpoint `
        --subscriptionKey $subscriptionKey `
        --versionId $version `
        --json 2>> $log | ConvertFrom-Json

    if ($result.Status -eq 'Success') {
        Write-Host "Done." -ForegroundColor Green
    }
    else {
        Write-Host "Error." -ForegroundColor Red
        Write-Host "! Could not import new version. Please review your LUIS application in the LUIS portal to resolve any issues." -ForegroundColor Red
        Break
    }

    Write-Host "> Training and publishing LUIS app ..." -NoNewline
    $(bf luis:train:run `
        --appId $luisApp.id `
        --endpoint $endpoint `
        --subscriptionKey $subscriptionKey `
        --versionId $luisApp.activeVersion `
        --wait
    & bf luis:application:publish `
        --appId $luisApp.id `
        --endpoint $endpoint `
        --subscriptionKey $subscriptionKey `
        --versionId $luisApp.activeVersion) 2>> $log | Out-Null
    Write-Host "Done." -ForegroundColor Green
}

function RunLuisGen($luFile, $outName, $outFolder, $log)
{
    $id = $luFile.BaseName
	$luisFolder = $luFile.DirectoryName
	$luisFile = Join-Path $luisFolder "$($id).json"

	bf luis:generate:cs `
        --in $luisFile `
        --className "$($outName)Luis" `
        --out $outFolder `
        --force 2>> $log | Out-Null
}
