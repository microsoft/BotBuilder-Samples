Param(
	[string] [Parameter(Mandatory=$true)]$Prefix,
	[string] [Parameter(Mandatory=$true)]$InputPath,
    [string] [Parameter(Mandatory=$true)]$OutputPath,
    [string] [Parameter(Mandatory=$true)]$BotFilePath,
    [string] [Parameter(Mandatory=$true)]$AuthoringKey,
	[string] [Parameter(Mandatory=$true)]$Endpoint,
	[string] [Parameter(Mandatory=$true)]$SubscriptionKey
)

Set-PSDebug -Off;

$luFiles = get-childitem $InputPath -recurse | where {$_.extension -eq ".lu"}

foreach($lu in $luFiles)
{
	Write-Host "Creating LUIS app from $($lu)..."

	$FileName = "$($Prefix)_$($lu.Basename)"

	# Create Json
	$luDownResult = ludown parse toluis --in "$($InputPath)\$($lu)" -o $OutputPath -n $FileName

	# Create luisgen file
	$lgResult = luisgen "$($OutputPath)\$($FileName).json" -cs $FileName -o 

	# Create luis app
	$str = luis import application --in "$($OutputPath)\$($FileName).json" --appName $FileName --authoringKey $AuthoringKey --endpointBasePath $Endpoint
    $json = "$str".Substring(0, "$str".Length / 2) | ConvertFrom-Json


	# Train and publish
	$pubResult = luis train version --applicationId $json."id" --versionId 0.1 --authoringKey $AuthoringKey --endpointBasePath $Endpoint --wait 
	& luis publish version --applicationId $json."id" --versionId 0.1 --authoringKey $AuthoringKey --endpointBasePath $Endpoint --subscriptionKey $SubscriptionKey

	# Connect to .bot file
	$luisObj = msbot connect luis --bot $BotFilePath --appId $json."id" --version 0.1 --authoringKey $AuthoringKey --subscriptionKey $SubscriptionKey --name $FileName | ConvertFrom-Json
}

Write-Host "LUIS Deployment Complete."