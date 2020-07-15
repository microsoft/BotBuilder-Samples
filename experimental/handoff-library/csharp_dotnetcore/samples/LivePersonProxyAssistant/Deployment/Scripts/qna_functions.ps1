function DeployKB ($name, $luFile, $qnaSubscriptionKey, $qnaEndpoint, $language, $log)
{
    $id = $luFile.BaseName
    $outFile = "$($id).json"
    $outFolder = $luFile.DirectoryName

    # Parse LU file
    Write-Host "> Converting $($language) $($id) QnA file ..." -NoNewline
	bf qnamaker:convert `
        --in $luFile `
        --out $(Join-Path $outFolder $outFile) `
        --force 2>> $log | Out-Null
    Write-Host "Done." -ForegroundColor Green
        
	# Create QnA Maker kb
    Write-Host "> Deploying $($language) $($id) QnA kb ..." -NoNewline

	# These values pretty much guarantee success. We can decrease them if the QnA backend gets faster
    $initialDelaySeconds = 60;
    $retryAttemptsRemaining = 4;
    $retryDelaySeconds = 15;
    $retryDelayIncrease = 30;

    while ($retryAttemptsRemaining -ge 0) {
		$bfconfig = (bf qnamaker:kb:create `
			--name $name `
			--subscriptionKey $qnaSubscriptionKey `
            --endpoint $qnaEndpoint `
			--in $(Join-Path $outFolder $outFile) | ConvertFrom-Json) 2>> $log

		if (-not $bfconfig.kbId) {
			$retryAttemptsRemaining = $retryAttemptsRemaining - 1
			Write-Host $retryAttemptsRemaining
			Start-Sleep -s $retryDelaySeconds
			$retryDelaySeconds += $retryDelayIncrease

			if ($retryAttemptsRemaining -lt 0) {
				Write-Host "! Unable to create QnA KB." -ForegroundColor Cyan
			}
			else {
				Write-Host "> Retrying ..."
				Continue
			}
		}
		else {
			Break
		}
    }

	if (-not $bfconfig.kbId) {
		Write-Host "! Could not deploy knowledgebase. Review the log for more information." -ForegroundColor DarkRed
		Write-Host "! Log: $($log)" -ForegroundColor DarkRed
		Return $null
	}
	else {
	    # Publish QnA Maker knowledgebase
        Write-Host "Done." -ForegroundColor Green
		bf qnamaker:kb:publish `
            --kbId $bfconfig.kbId `
            --subscriptionKey $qnaSubscriptionKey `
            --endpoint $qnaEndpoint 2>> $log | Out-Null

		Return $bfconfig
	}
}

function UpdateKB ($luFile, $kbId, $qnaSubscriptionKey, $qnaEndpoint, $language, $log)
{
    $id = $luFile.BaseName
    $outFile = "$($id).json"
    $outFolder = $luFile.DirectoryName

    # Parse LU file
    Write-Host "> Converting $($language) $($id) QnA file ..." -NoNewline
	bf qnamaker:convert `
        --in $luFile `
        --out $(Join-Path $outFolder $outFile) `
        --force 2>> $log | Out-Null
    Write-Host "Done." -ForegroundColor Green

    Write-Host "> Replacing $($language) $($id) QnA kb ..." -NoNewline
	bf qnamaker:kb:replace `
        --in $(Join-Path $outFolder $outFile) `
        --kbId $kbId `
        --subscriptionKey $qnaSubscriptionKey `
		--endpoint $qnaEndpoint 2>> $log | Out-Null

    # Publish QnA Maker knowledgebase
	bf qnamaker:kb:publish `
        --kbId $kbId `
        --endpoint $qnaEndpoint `
        --subscriptionKey $qnaSubscriptionKey 2>> $log | Out-Null

    Write-Host "Done." -ForegroundColor Green
}
