# Search Powered Bots Tools: Generate

The Generate tool is a console application that takes the output json from the Extract tool, and uses it to create and optionally publish a LUIS model specific to the domain corpus analyzed from the Azure Search Index by Search.Tools.Extract. The resulting LUIS Model can be used to create rich search powered conversational bots.

## Build and Publich

To build and publish this tool, the command below needs to be executed after specifying the mode and runtime parameters.

`dotnet publish -c <mode> -r <runtime>`

The mode can be either *debug* or *release*, and the runtime can be any of the supported runtimes, which can be found in the [.NET Core Runtime IDentifier (RID) catalog](https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog).

For example, running the following command in the Search.Tools.Generate folder will result in building in debug mode for the Windows 10 x64 runtime:

`dotnet publish -c debug -r win10-x64` 

## Usage

After publishing, navigate to the output directory and you can run the program. You need to have generated a schema file either by hand or by using Search.Tools.Extract.  For example, this is the command line to generate a LUIS model and then upload and publish it to LUIS. 
`Search.Tools.Generate RealEstate.json -o RealEstateModel.json  -s <BingSpellingKey> -l <LUISSubscriptionKey> -u`

The program works by combining the information found in the schema file with the LUIS app template file SearchTemplate.json found in this project.  
This file can also be imported to LUIS and changed and then exported and used by this program as the LUIS app model template.  The template file
is a regular LUIS application except that it has some placeholders including:
'keyword' for where a keyword can be found.
`attributeName' for where an attribute could be found. An attribute is something like "sold" where the value implies the field.
'moneyName' for where a currency related property name could be found.
'numberName' for where a numeric property name could be found.
'stringName' for where a string related property name could be found.

If no parameters are specified, the program will display detailed help and information about the parameters accepted like this:
`Search.Tools.Generate <schemaFile> [-d <LUIS Domain>] [-l <LUIS subscription key>] [-m <modelName>] [-o <outputFile>] [-ot <outputTemplate>] [-s <splleing API Key] [-tf <templateFile>] [-tm <modelName>] [-u] [-ut]
Take a JSON schema file and use it to generate a LUIS model from a template.
The template can be the included SearchTemplate.json file or can be downloaded from LUIS.
The resulting LUIS model can be saved as a file or automatically uploaded to LUIS.
-d <LUIS Domain> : LUIS domain which defaults to westus.api.cognitive.microsoft.com.
-l <LUIS subscription key> : LUIS subscription key, default is environment variable from LUISSubscriptionKey.
-m <modelName> : Output LUIS model name.  By default will be <schemaFileName>Model.
-o <outputFile> : Output LUIS JSON file to generate. By default this will be <schemaFileName>Model.json in the same directory as <schemaFile>.
-ot <outputFile> : Output template to <outputFile>.
-s <spelling API Key> : Enable spelling using supplied API key.
-tf <templateFile> : LUIS Template file to modify based on schema.  By default this is SearchTemplate.json.
-tm <modelName> : LUIS model to use as template. Must also specify -l.
-u: Upload resulting model to LUIS.  Must also specify -l.
-ut: Upload template to LUIS.  Must also specify -l.
Use {} to comment out arguments.
Common usage:
generate <schema> : Generate <schema>Model.json in the directory with <schema> from the SearchTemplate.json.
generate <schema> -l <LUIS key> -u : Update the existing <schemaFileName>Model LUIS model and upload it to LUIS.`

## Resources

### .NET Core
[.NET Core Getting Started](https://docs.microsoft.com/en-us/dotnet/articles/core/tutorials/using-with-xplat-cli)
[.NET Core Deployment](https://docs.microsoft.com/en-us/dotnet/articles/core/deploying/index)
[.NET Core Runtime IDentifier (RID) catalog](https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog).

### Azure Search

[Azure Search](https://azure.microsoft.com/en-us/services/search/) 

### LUIS

[LUIS](https://www.luis.ai/) 