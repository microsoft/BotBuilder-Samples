call ludown parse toluis --in Dialogs\RootDialog.lu -o CognitiveModels -n CoreBot --out CoreBot.luis.json --verbose
call luis rename version --appId 1195bcf1-4610-4285-982e-3a97cce409a2 --authoringKey a95d07785b374f0a9d7d40700e28a285 --versionId 0.1 --newVersionId 0.1_old 
call luis import version --appId 1195bcf1-4610-4285-982e-3a97cce409a2 --authoringKey a95d07785b374f0a9d7d40700e28a285 --in CognitiveModels\CoreBot.luis.json --versionId 0.1 
call luis delete version --appId 1195bcf1-4610-4285-982e-3a97cce409a2 --authoringKey a95d07785b374f0a9d7d40700e28a285 --versionId 0.1_old --force
call luis train version --appId 1195bcf1-4610-4285-982e-3a97cce409a2 --authoringKey a95d07785b374f0a9d7d40700e28a285 --versionId 0.1 --wait
call luis publish version --appId 1195bcf1-4610-4285-982e-3a97cce409a2 --authoringKey a95d07785b374f0a9d7d40700e28a285 --versionId 0.1 --wait


