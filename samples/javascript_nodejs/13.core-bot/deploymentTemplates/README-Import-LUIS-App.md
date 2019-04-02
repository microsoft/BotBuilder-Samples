## Import a new LUIS Application using a local LUIS application

## Prerequisites:

#### Install LUIS CLI >=2.4.0:
Open a CLI of your choice and type the following:
```bash
npm i -g luis-apis@^2.4.0
```

#### LUIS portal account:
You should already have a LUIS account with either https://luis.ai, https://eu.luis.ai, or https://au.luis.ai. To determine where to create a LUIS account, consider where you will deploy your LUIS applications, and then place them in [the corresponding region][LUIS-Authoring-Regions].

After you've created your account, you need your [Authoring Key][LUIS-AKey] and a LUIS application ID.

  [LUIS-Authoring-Regions]: https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-reference-regions#luis-authoring-regions]
  [LUIS-AKey]: https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-concept-keys#authoring-key

___

### 1. Import the local LUIS application to luis.ai
```bash
luis import application --region "LuisAppAuthoringRegion" --authoringKey "LuisAuthoringKey" --appName "CoreBot-FlightBooking" --in "./cognitiveModels/FlightBooking.json"
```

Outputs the following JSON:
```json
{
    "id": "########-####-####-####-############",
    "name": "CoreBot-FlightBooking",
    "description": "A LUIS model that uses intent and entities.",
    "culture": "en-us",
    "usageScenario": "",
    "domain": "",
    "versionsCount": 1,
    "createdDateTime": "2019-03-29T18:32:02Z",
    "endpoints": {},
    "endpointHitsCount": 0, 
    "activeVersion": "0.1",
    "ownerEmail": "bot@contoso.com",
    "tokenizerVersion": "1.0.0"
}
```

For the next step, you'll need the `"id"` value for `--appId` and the `"activeVersion"` value for `--versionId`.

### 2. Train the LUIS Application
```bash
luis train version --region "LuisAppAuthoringRegion" --authoringKey "LuisAuthoringKey" --appId "LuisAppId" --versionId "LuisAppversion" --wait
```

### 3. Publish the LUIS Application
```bash
luis publish version --region "LuisAppAuthoringRegion" --authoringKey "LuisAuthoringKey" --appId "LuisAppId" --versionId "LuisAppversion" --publishRegion "LuisAppPublishRegion"
```

> `--region` corresponds to the region you _author_ your application in. The regions available for this are "westus", "westeurope" and "australiaeast". <br/>
> These regions correspond to the three available portals, https://luis.ai, https://eu.luis.ai, or https://au.luis.ai. <br/>
> `--publishRegion` corresponds to the region of the endpoint you're publishing to, (e.g. "westus", "southeastasia", "westeurope", "brazilsouth"). <br/>
> See the [reference docs][Endpoint-API] for a list of available publish/endpoint regions.

  [Endpoint-API]: https://westus.dev.cognitive.microsoft.com/docs/services/5819c76f40a6350ce09de1ac/operations/5819c77140a63516d81aee78


Outputs the following:
```json
 {
    "versionId": "0.1",
    "isStaging": false,
    "endpointUrl": "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/########-####-####-####-############",
    "region": "westus",
    "assignedEndpointKey": null,
    "endpointRegion": "westus",
    "failedRegions": "",
    "publishedDateTime": "2019-03-29T18:40:32Z",
    "directVersionPublish": false
}
```

To see how to create an LUIS Cognitive Service Resource in Azure, please see [the next README][README-LUIS]. This Resource should be used when you want to move your bot to production. The instructions will show you how to create and pair the resource with a LUIS Application.

  [README-LUIS]: ./README-LUIS.md