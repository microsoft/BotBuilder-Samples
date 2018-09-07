
#---------------------Variables-----------------------------------------
$serviceName = Read-Host -Prompt 'Service Name (this is also the name of your resource group)'
$luisAuthoringKey = Read-Host -Prompt 'Input your LUIS authoring key' # Authoring key cannot be accessed via API

# Get file path for .bot file
$botDir = "$PSScriptRoot\..\"
$botFiles = get-childitem $botDir -recurse | where {$_.extension -eq ".bot"}
$botFile = $botFiles[0]
$botPath = "$PSScriptRoot\..\$botFile"

$luisJsonPath = "$PSScriptRoot\..\CogSvcModels\Dispatch"
$luisJsonPath = "$PSScriptRoot\..\CogSvcModels\LUIS"
$qnaJsonPath = "$PSScriptRoot\..\CogSvcModels\QnAMaker"


#---------------------Reset .Bot and Working Folder-----------------------

Get-ChildItem $modelOutputPath -Include * -Recurse | Remove-Item

$msbot = Get-Content -Raw -Path $botPath | ConvertFrom-Json
$dispatchModels = $msbot.services | where type -eq "dispatch" 
$luisModels = $msbot.services | where type -eq "luis" 
$qnaModels = $msbot.services | where type -eq "qna" 

foreach($d in $dispatchModels)
{
    msbot disconnect --bot $botPath $d.name
}

foreach($l in $luisModels)
{
    msbot disconnect --bot $botPath $l.name
}

foreach($q in $qnaModels)
{
    msbot disconnect --bot $botPath $q.name
}


#-------------------------ARM Deployment---------------------------

Invoke-Expression "$PSScriptRoot\Deploy-AzureResourceGroup.ps1 -ResourceGroupName $serviceName"

$lastDeployment = Get-AzureRmResourceGroupDeployment -ResourceGroupName $serviceName | Sort Timestamp -Descending | Select -First 1 

if(!$lastDeployment) {
    throw "Deployment could not be found for Resource Group '$serviceName'."
}

if(!$lastDeployment.Outputs) {
    throw "No output parameters could be found for the last deployment of Resource Group $serviceName'."
}

$appInsightsResourceId = $lastDeployment.Outputs.Item("appInsightsResourceId").Value
$luisResourceId = $lastDeployment.Outputs.Item("luisResourceId").Value
$qnaResourceId = $lastDeployment.Outputs.Item("qnaMakerResourceId").Value
$contentModeratorResourceId = $lastDeployment.Outputs.Item("contentModeratorResourceId").Value
$storageResourceId = $lastDeployment.Outputs.Item("storageResourceId").Value
$cosmosDbResourceId = $lastDeployment.Outputs.Item("cosmosDbResourceId").Value

Write-Host "Extracting service keys..."

$appInsights = Get-AzureRmResource -ResourceId $appInsightsResourceId
$appInsightsKey = $appInsights.Properties.InstrumentationKey

$luis = Get-AzureRmResource -ResourceId $luisResourceId 
$luisEndpoint = $luis.Properties.endpoint
$luisLocation = $luis.Location
$luisKeys = Invoke-AzureRmResourceAction -Action ListKeys -ResourceId $luisResourceId -Force
$luisSubscriptionKey = $luisKeys.key1


$qna = Get-AzureRmResource -ResourceId $qnaResourceId
$qnaHostName = $qna.Properties.apiProperties.qnaRuntimeEndpoint
$qnaKeys = Invoke-AzureRmResourceAction -Action ListKeys -ResourceId $qnaResourceId -Force
$qnaSubscriptionKey = $qnaKeys.key1


$contentModerator = Get-AzureRmResource -ResourceId $contentModeratorResourceId
$contentModeratorRegion = $contentModerator.Location
$contentModeratorKeys = Invoke-AzureRmResourceAction -Action ListKeys -ResourceId $contentModeratorResourceId -Force
$contentModeratorKey = $contentModeratorKeys.key1


$storage = Get-AzureRmResource -ResourceId $storageResourceId 
$storageAccountKey = (Get-AzureRmStorageAccountKey -ResourceGroupName $serviceName -Name $storage.Name).Value[0]
$storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=$($storage.Name);AccountKey=$($storageAccountKey);EndpointSuffix=core.windows.net"


$cosmosdb = Get-AzureRmResource -ResourceId $cosmosDbResourceId
$cosmosEndpoint = $cosmosdb.Properties.documentEndpoint
$cosmosKeys = Invoke-AzureRmResourceAction -Action ListKeys -ResourceId $cosmosDbResourceId -Force
$cosmosKey = $cosmosKeys.primaryMasterKey


#-------------------------LUIS-------------------------------
Invoke-Expression "$PSScriptRoot\Cognitive Services\Deploy-Luis.ps1 -Prefix $serviceName -InputPath $luisInputPath -OutputPath $luisOutputPath -BotFilePath $botPath -AuthoringKey $luisAuthoringKey -Endpoint $luisEndpoint -SubscriptionKey $luisSubscriptionKey"

#-------------------------QnAMaker---------------------------
Invoke-Expression "$PSScriptRoot\Cognitive Services\Deploy-QnAMaker.ps1 -Prefix $serviceName -InputPath $qnaInputPath -OutputPath $qnaOutputPath -BotFilePath $botPath -Hostname $qnaHostName -SubscriptionKey $qnaSubscriptionKey"

#-------------------------Dispatch---------------------------
Invoke-Expression "$PSScriptRoot\Cognitive Services\Deploy-Dispatch.ps1 -Prefix $serviceName -OutputPath $dispatchOutputPath -BotFilePath $botPath -AuthoringKey $luisAuthoringKey -Endpoint $luisEndpoint -SubscriptionKey $luisSubscriptionKey -Location $luisLocation"

#--------------------Connected Services----------------------
msbot connect cosmosdb --bot $botPath -serviceName $cosmosdb.Name -endpoint $cosmosEndpoint -key $cosmosKey -database "" -collection "" 

msbot connect blob --bot $botPath --serviceName $storage.Name -connectionString $storageConnectionString --container ""

msbot connect appinsights  --bot $botPath --serviceName $appInsights.Name --instrumentationKey $appInsights.InstrumentationKey

msbot connect generic --bot $botPath --name "ContentModerator" --keys { "SubscriptionKey" : $contentModeratorKey, "region" : "$contentModeratorRegion" }