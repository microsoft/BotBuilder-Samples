call ludown parse toluis --in Dialogs\RootDialog\RootDialog.lu -o CognitiveModels -n ToDoLuisBot --out ToDoLuisBot.luis.json --verbose
call luis rename version --appId d5759ad1-2372-42d8-9eb2-c168ba9b566e --versionId 0.1 --newVersionId 0.1_old --authoringKey a95d07785b374f0a9d7d40700e28a285
call luis import version --in CognitiveModels\ToDoLuisBot.luis.json --appId d5759ad1-2372-42d8-9eb2-c168ba9b566e --versionId 0.1 --authoringKey a95d07785b374f0a9d7d40700e28a285
call luis delete version --appId d5759ad1-2372-42d8-9eb2-c168ba9b566e --versionId 0.1_old --authoringKey a95d07785b374f0a9d7d40700e28a285 --force
call luis train version --appId d5759ad1-2372-42d8-9eb2-c168ba9b566e --versionId 0.1 --authoringKey a95d07785b374f0a9d7d40700e28a285 --wait
call luis publish version --appId d5759ad1-2372-42d8-9eb2-c168ba9b566e --versionId 0.1 --authoringKey a95d07785b374f0a9d7d40700e28a285 --wait