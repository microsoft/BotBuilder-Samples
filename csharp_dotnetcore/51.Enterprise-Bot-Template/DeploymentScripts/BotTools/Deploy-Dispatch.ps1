Param(
	[string] [Parameter(Mandatory=$true)]$Prefix,
    [string] [Parameter(Mandatory=$true)]$OutputPath,
    [string] [Parameter(Mandatory=$true)]$BotFilePath,
    [string] [Parameter(Mandatory=$true)]$AuthoringKey,
	[string] [Parameter(Mandatory=$true)]$Location,
	[string] [Parameter(Mandatory=$true)]$SubscriptionKey
)

Write-Host "Creating Dispatch model..."

$FileName = "$($Name)_Dispatch"

# Create dispatch service & connect to .bot file
$dispatchObj = dispatch create --name $FileName --bot $BotFilePath --luisAuthoringRegion $luisLocation --luisAuthoringKey $luisAuthoringKey --subscriptionKey $SubscriptionKey --dataFolder $OutputPath | msbot connect dispatch --stdin --bot $BotFilePath | ConvertFrom-Json

# Create luisgen file
$lgResult = luisgen "$($OutputPath)\$($FileName).json" -cs $FileName -o 

Write-Host $dispatchObj

Write-Host "Dispatch Deployment Complete."