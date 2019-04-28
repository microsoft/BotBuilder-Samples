call ludown parse toluis --in Dialogs\MainAdaptiveDialog.lu -o CognitiveModels -n CoreBot --out CoreBot.luis.json --verbose
call luis rename version --appId 1195bcf1-4610-4285-982e-3a97cce409a2 --versionId 0.1 --newVersionId 0.1_old --authoringKey a95d07785b374f0a9d7d40700e28a285
call luis import version --in CognitiveModels\CoreBot.luis.json --appId 1195bcf1-4610-4285-982e-3a97cce409a2 --versionId 0.1 --authoringKey a95d07785b374f0a9d7d40700e28a285
call luis delete version --appId 1195bcf1-4610-4285-982e-3a97cce409a2 --versionId 0.1_old --authoringKey a95d07785b374f0a9d7d40700e28a285 --force
call luis train version --appId 1195bcf1-4610-4285-982e-3a97cce409a2 --versionId 0.1 --authoringKey a95d07785b374f0a9d7d40700e28a285 --wait
call luis publish version --appId 1195bcf1-4610-4285-982e-3a97cce409a2 --versionId 0.1 --authoringKey a95d07785b374f0a9d7d40700e28a285 --wait


