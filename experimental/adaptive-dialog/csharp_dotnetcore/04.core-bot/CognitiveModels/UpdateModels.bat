call bf luis:convert --in ./Dialogs/RootDialog.lu --out ./CognitiveModels/CoreBot.luis.json --force
call luis rename version --appId <LUIS-APP-ID> --authoringKey <AUTHORING-KEY> --versionId 0.1 --newVersionId 0.1_old 
call luis import version --appId <LUIS-APP-ID> --authoringKey <AUTHORING-KEY> --in CognitiveModels\CoreBot.luis.json --versionId 0.1
call luis delete version --appId <LUIS-APP-ID> --authoringKey <AUTHORING-KEY> --versionId 0.1_old --force
call luis train version --appId <LUIS-APP-ID> --authoringKey <AUTHORING-KEY> --versionId 0.1 --wait
call luis publish version --appId <LUIS-APP-ID> --authoringKey <AUTHORING-KEY> --versionId 0.1 --wait