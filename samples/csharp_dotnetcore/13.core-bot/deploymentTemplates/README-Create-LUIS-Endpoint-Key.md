## How to create a LUIS Endpoint resource in Azure and pair it with a LUIS Application

### Prerequisites:

#### Install Azure CLI >=2.0.61:
Visit the following page to find the correct installer for your OS:
- https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?view=azure-cli-latest


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

### 1. Create a new LUIS Cognitive Services resource on Azure via Azure CLI

> _Note:_ <br/>
> _If you don't have a Resource Group in your Azure subscription, you can create one through the Azure portal or through using:_
> ```bash
> az group create --subscription "AzureSubscriptionGuid" --location "westus" --name "ResourceGroupName"
> ```
> _To see a list of valid locations, use `az account list-locations`_


```bash
# Use Azure CLI to create the LUIS Key resource on Azure
az cognitiveservices account create --kind "luis" --name "NewLuisResourceName" --sku "S0" --location "westus" --subscription "AzureSubscriptionGuid" -g "ResourceGroupName"
```

The command will output a response similar to the JSON below:
```json
{
  "endpoint": "https://westus.api.cognitive.microsoft.com/luis/v2.0",
  "etag": "\"########-####-####-####-############\"",
  "id": "/subscriptions/########-####-####-####-############/resourceGroups/ResourceGroupName/providers/Microsoft.CognitiveServices/accounts/NewLuisResourceName",
  "internalId": "################################",
  "kind": "luis",
  "location": "westus",
  "name": "NewLuisResourceName",
  "provisioningState": "Succeeded",
  "resourceGroup": "ResourceGroupName",
  "sku": {
    "name": "S0",
    "tier": null
  },
  "tags": null,
  "type": "Microsoft.CognitiveServices/accounts"
}
```



Take the output from the previous command and create a JSON file in the following format:
```json
{
    "azureSubscriptionId": "00000000-0000-0000-0000-000000000000",
    "resourceGroup": "ResourceGroupName",
    "accountName": "NewLuisResourceName"
}
```

### 2. Retrieve ARM access token via Azure CLI
```bash
az account get-access-token --subscription "AzureSubscriptionGuid"
```

This will return an object that looks like this:
```json
{
  "accessToken": "eyJ0eXAiOiJKVtokentokentokentokentokeng1dCI6Ik4tbEMwbi05REFMcXdodUhZbkhRNjNHZUNYYyIsItokenI6Ik4tbEMwbi05REFMcXdodUhZbkhRNjNHZUNYYyJ9.eyJhdWQiOiJodHRwczovL21hbmFnZW1lbnQuY29yZS53aW5kb3dzLm5ldC8iLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC83MmY5ODhiZi04NmYxLTQxYWYtOTFhYi0yZDdjZDAxMWRiNDcvIiwiaWF0IjoxNTUzODc3MTUwLCJuYmYiOjE1NTM4NzcxNTAsImV4cCI6MTU1Mzg4MTA1MCwiX2NsYWltX25hbWVzIjp7Imdyb3VwcyI6InNyYzEifSwiX2NsYWltX3NvdXJjZXMiOnsic3JjMSI6eyJlbmRwb2ludCI6Imh0dHBzOi8vZ3JhcGgud2luZG93cy5uZXQvNzJmOTg4YmYtODZmMS00MWFmLTkxYWItMmQ3Y2QwMTFkYjQ3L3VzZXJzL2ZmZTQyM2RkLWJhM2YtNDg0Ny04NjgyLWExNTI5MDA4MjM4Ny9nZXRNZW1iZXJPYmplY3RzIn19LCJhY3IiOiIxIiwiYWlvIjoiQVZRQXEvOEtBQUFBeGVUc201NDlhVHg4RE1mMFlRVnhGZmxxOE9RSC9PODR3QktuSmRqV1FqTkkwbmxLYzB0bHJEZzMyMFZ5bWZGaVVBSFBvNUFFUTNHL0FZNDRjdk01T3M0SEt0OVJkcE5JZW9WU0dzd0kvSkk9IiwiYW1yIjpbIndpYSIsIm1mYSJdLCJhcHBpZCI6IjA0YjA3Nzk1LThkZGItNDYxYS1iYmVlLTAyZjllMWJmN2I0NiIsImFwcGlkYWNyIjoiMCIsImRldmljZWlkIjoiNDhmNDVjNjEtMTg3Zi00MjUxLTlmZWItMTllZGFkZmMwMmE3IiwiZmFtaWx5X25hbWUiOiJHdW0iLCJnaXZlbl9uYW1lIjoiU3RldmVuIiwiaXBhZGRyIjoiMTY3LjIyMC4yLjU1IiwibmFtZSI6IlN0ZXZlbiBHdW0iLCJvaWQiOiJmZmU0MjNkZC1iYTNmLTQ4NDctODY4Mi1hMTUyOTAwODIzODciLCJvbnByZW1fc2lkIjoiUy0xLTUtMjEtMjEyNzUyMTE4NC0xNjA0MDEyOTIwLTE4ODc5Mjc1MjctMjYwOTgyODUiLCJwdWlkIjoiMTAwMzdGRkVBMDQ4NjlBNyIsInJoIjoiSSIsInNjcCI6InVzZXJfaW1wZXJzb25hdGlvbiIsInN1YiI6Ik1rMGRNMWszN0U5ckJyMjhieUhZYjZLSU85LXVFQVVkZFVhNWpkSUd1Nk0iLCJ0aWQiOiI3MmY5ODhiZi04NmYxLTQxYWYtOTFhYi0yZDdjZDAxMWRiNDciLCJ1bmlxdWVfbmFtZSI6InN0Z3VtQG1pY3Jvc29mdC5jb20iLCJ1cG4iOiJzdGd1bUBtaWNyb3NvZnQuY29tIiwidXRpIjoiT2w2NGN0TXY4RVNEQzZZQWRqRUFtokenInZlciI6IjEuMCJ9.kFAsEilE0mlS1pcpqxf4rEnRKeYsehyk-gz-zJHUrE__oad3QjgDSBDPrR_ikLdweynxbj86pgG4QFaHURNCeE6SzrbaIrNKw-n9jrEtokenlosOxg_0l2g1LeEUOi5Q4gQREAU_zvSbl-RY6sAadpOgNHtGvz3Rc6FZRITfkckSLmsKAOFoh-aWC6tFKG8P52rtB0qVVRz9tovBeNqkMYL49s9ypduygbXNVwSQhm5JszeWDgrFuVFHBUP_iENCQYGQpEZf_KvjmX1Ur1F9Eh9nb4yI2gFlKncKNsQl-tokenK7-tokentokentokentokentokentokenatoken",
  "expiresOn": "2200-12-31 23:59:59.999999",
  "subscription": "AzureSubscriptionGuid",
  "tenant": "tenant-guid",
  "tokenType": "Bearer"
}
```

The value needed for the next step is the `"accessToken"`.


### 3. Use `luis add appazureaccount` to pair your LUIS resource with a LUIS Application

```bash
luis add appazureaccount --in "path/to/created/requestBody.json" --appId "LuisAppId" --authoringKey "LuisAuthoringKey" --armToken "accessToken"
```

If successful, it should yield a response like this:
```json
{
  "code": "Success",
  "message": "Operation Successful"
}
```

### 4. See the LUIS Cognitive Services' keys

```bash
az cognitiveservices account keys list --name "NewLuisResourceName" --subscription "AzureSubscriptionGuid" -g "ResourceGroupName"
```

This will return an object that looks like this:
```json
{
  "key1": "9a69####dc8f####8eb4####399f####",
  "key2": "####f99e####4b1a####fb3b####6b9f"
}
```

  [LUIS-NPM]: https://www.npmjs.com/package/luis-apis

