Param(
	[string] [Parameter(Mandatory=$true)]$Prefix,
	[string] [Parameter(Mandatory=$true)]$InputPath,
    [string] [Parameter(Mandatory=$true)]$OutputPath,
    [string] [Parameter(Mandatory=$true)]$BotFilePath,
	[string] [Parameter(Mandatory=$true)]$Hostname,
	[string] [Parameter(Mandatory=$true)]$SubscriptionKey
)


$qnaFiles = get-childitem $InputPath -recurse | where {$_.extension -eq ".lu"}

foreach($qna in $qnaFiles)
{
    Write-Host "Creating QnAMaker knowledge base from $($qna)..."

	$FileName = "$($Prefix)_$($qna.Basename)"

    # parse .lu files into json
    ludown parse toqna --in "$($InputPath)\$($qna)" -o $OutputPath -n $FileName

    # create knowledgebase
    $qnaObj = qnamaker create kb --in "$($OutputPath)\$($FileName).json" --name $FileName --subscriptionKey $SubscriptionKey --hostName $Hostname --msbot |  msbot connect qna --stdin --bot $BotFilePath | ConvertFrom-Json
 

    # Publish KB
    qnamaker publish kb --kbId $qnaObj."kbId" --subscriptionKey $SubscriptionKey --hostName $Hostname
}

Write-Host "QnAMake Deployment Complete."