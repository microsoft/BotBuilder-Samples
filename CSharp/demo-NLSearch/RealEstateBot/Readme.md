In order to generate a LUIS application for RealEstateBot.
1) Extract schema information from Azure Search: Search.Tools.Extract realestate listings 93B04DA93FF693841A35B66AF9D32023 -g RealEstateBot-histogram.bin -v price -h RealEstateBot-histogram.bin -o ..\..\..\RealEstateBot\Dialogs\RealEstateBot.json 
2) Set the environment variable LUISSubscriptionKey = <your LUIS subscription key> or specify with -l.
3) Generate and upload the model with: Search.Tools.Generate ..\..\RealEstateBot\Dialogs\RealEstateBot.json -l <your LUIS Key> -o ..\..\RealEstateBot\Dialogs\RealEstateModel.json  -u