A sample bot that shows how to use LUIS and Azure Search to support picking a list of matches from a real estate database through queries like "3+ bedroom house in seattle with a fireplace for less than $800k".  Projects included in this sample:
* __Search.Tools.Extract__ : A tool for analyzing an Azure Search index in order to generate a description of the schema and contents of the index.
* __Search.Tools.Generate__ : A tool for combining the meta-data with a LUIS template that supports comparisons to make a custom LUIS model.
* __Search.Dialogs__ : A generic Bot Control that uses the custom LUIS model to interpret user conversation for finding matches in an Azure Search database.  The control is multi-turn and supports both guided and freefrom interactions.
* __RealEstateBot__ : A sample bot that utilitizes the Azure Search Bot control to select matches from an Azure Search sample database listing of 5 million items.

In order to use this on your own Azure Search database.  (You can find more information on each tool in the readme for those projects.)
1) Extract schema information from Azure Search: `Search.Tools.Extract <Azure Search Index> <Azure Search Service Key -g <.bin file name for histogram> -v <order by field when using -g> -h <.bin file for histogram> -o <json output location>` 
Search.Tools.Extract realestate listings 93B04DA93FF693841A35B66AF9D32023 -g RealEstateBot-histogram.bin -v price -h RealEstateBot-histogram.bin -o ..\..\..\RealEstateBot\Dialogs\RealEstateBot.json 
2) Set the environment variable LUISSubscriptionKey = <your LUIS subscription key> or specify with -l.
3) Generate and upload the model with: Search.Tools.Generate ..\..\RealEstateBot\Dialogs\RealEstateBot.json -l <your LUIS Key> -o ..\..\RealEstateBot\Dialogs\RealEstateModel.json  -u

