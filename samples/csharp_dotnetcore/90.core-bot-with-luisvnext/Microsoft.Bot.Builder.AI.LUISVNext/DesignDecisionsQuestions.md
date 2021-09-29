# Bot Builder - Recognizer Extensions Vital Design Decisions

## LUIS Entities vs LUISVNext entities

### Context
As per the [LUISVNext Swagger](https://github.com/Azure/azure-rest-api-specs/blob/ee3061aa89dcb77829a327cf159fbb9c12ea013f/specification/cognitiveservices/data-plane/Language/preview/2021-05-01-preview/analyzeconversations.json) and the actual behavior of the service, LUISVNext returns detected entities in its response as shown below:

```
"query": "I want to book a flight from London to Cairo",
"prediction": {
    ...
    "entities": [
                {
                    "category": "fromCity",
                    "text": "London",
                    "offset": 29,
                    "length": 6,
                    "confidenceScore": 0.81079376
                },
                {
                    "category": "toCity",
                    "text": "Cairo",
                    "offset": 39,
                    "length": 5,
                    "confidenceScore": 0.5041834
                }
            ],
   }
```

Instead of having the same structure of a LUIS entity object, LUISVNext simply returns a list of detected entities with each of their respective labels, location in the query and confidence score. For reference, this is what a LUIS response for a similar query might look like (with composite entities supported):

```
"query": "I want to book a flight from London to Cairo",
"prediction": {
    ...
    "entities": {
                "From": [
                    {}
                ],
                "To": [
                    {
                        "Airport": [
                            [
                                "Paris"
                            ]
                        ],
                        "$instance": {
                            "Airport": [
                                {
                                    "type": "Airport",
                                    "text": "paris",
                                    "startIndex": 28,
                                    "length": 5,
                                    "modelTypeId": 5,
                                    "modelType": "List Entity Extractor",
                                    "recognitionSources": [
                                        "model"
                                    ]
                                }
                            ]
                        }
                    }
                ],
                "$instance": {
                    "From": [
                        {
                            "type": "From",
                            "text": "egypt",
                            "startIndex": 19,
                            "length": 5,
                            "score": 0.9808726,
                            "modelTypeId": 4,
                            "modelType": "Composite Entity Extractor",
                            "recognitionSources": [
                                "model"
                            ]
                        }
                    ],
                    "To": [
                        {
                            "type": "To",
                            "text": "paris",
                            "startIndex": 28,
                            "length": 5,
                            "score": 0.99286765,
                            "modelTypeId": 4,
                            "modelType": "Composite Entity Extractor",
                            "recognitionSources": [
                                "model"
                            ]
                        }
                    ]
                }
   }
```

Since LUISVNext does not support composite entities, the response is much simpler. LUISVNext is also currently working on integrating pre-built and list entities, which will result in a change in the swagger (and possibly the format of the returned response).

### Issue and Possible solutions

The issue is as follows: A `RecognizerResult` contains a property `RecognizerResult.Entities`, which is described by the docstring as an "Object (JObject) with each top-level recognized entity as a key." Looking at the values within this object in the case of LUISRecognizer, we see that it looks much like a LUISResponse, which is quite different to that of a LUISVNext response.

There are three proposed solutions to this (please feel free to suggest more):

- Abandon using `RecognizerResult`, much like external recognizers that implement their own result definitions.
  - PRO: allows flexibility in represtening LUISVNext results in whatever format is most appropriate.
  - CON: Will be inconsistent to most if not all internal recognizers
- Reshape LUISVNext results to fit into the expected format
  - PRO: Consitency of formatting across BF SDK
  - CON: May be prone to change after swagger update
  - CON: Adding complexity to LUISVNext's simplified entity representation, which is not accurately representative of Service response.
- Return LUISVNext response within `RecognizerResult.Entities` Object as a simple list of entities (like the VNext response shown in the beggining of this section).
  - PRO: Easy to implement and modify later.
  - PRO: Entity list is easy to understand and consume, which is representative of the service behavior.
  - CON: Inconsistent with previous LUIS `RecognizerResult.Entities` object.


The decision made in the preliminary stages of development of this Recognizer is to go with the third option.

## LuisVNext recognizer in composer

Effort was made in integrating the tentative LuisVNext Recognizer as a package for composer to consume. The most vital functionality (Calling the APIs and returning a response) is now available in the package that can be exported from the project in this repository. However the following issues remain:

- How will the user provide his own LUISVNext application? This can be done in one of two ways:
  - Extend composer with functionality to author new LUISVNext applications. (I'm unsure if this is possible using external composer packages)
    - LUISVNext does not yet support LU files. It will require some effort for the package to accomodate this.
  - Provide a way for the user to input their own application's credentials such that they are available to the recognizer package. Our first intuition was to add set of fields in the UI for the user, but this option does not seem to be available for Recognizers.

- The decision made from the previous section regarding the formatting of entity lists makes the returned result awkward to use in Composer. Will this be a problem?

- Using the command "bf dialog:merge" does not automatically merge Microsoft.LuisVNext.uischema file with composer project's sdk.uischema file. However, it works fine with the regular .schema files. 

## New LuisApplication class

The main reason for this is that the [LuisApplication class in the old recognizer](https://github.com/microsoft/botbuilder-dotnet/blob/bf9d4f106ec8ffde9c4d274d974856e0e7b51220/libraries/Microsoft.Bot.Builder.AI.LUIS/LuisApplication.cs#L57) performs validation on the `LuisAppId` that does not apply LuisVNext applications. 

As of the preview release (API Version: "2021-07-15-preview") of LUISVNext, the application name is used to identify the LuisVNext project (in addition to the endpoint) and as such the `LuisAppId` is actually a string representing the name of the project. The old implementation of LUISApplication throws an error when trying to validate that `LuisAppId` is a GUID.


## DateTime in Sample not supported yet

The LUISVNext team is currently working on integrating prebuilt and list entities into the LUIVNext service. Since DateTime entities are usually best implemented as Prebuilt entities, it is decided to wait until the next release of the service to include this feature in our sample.
