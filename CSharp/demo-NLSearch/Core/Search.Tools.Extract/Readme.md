# Search Powered Bots Tools: Search.Tools.Extract

The Search.Tools.Extract program generates a histogram and computes different attributes and properties of an Azure Search index. The output of this program is then passed to the Generate program, to jointly generate a LUIS model for Search Powered Bots.

## Build and Publish

To build and publish this tool, the command below needs to be executed after specifying the mode and runtime parameters.

`dotnet publish -c <mode> -r <runtime>`

The mode can be either *debug* or *release*, and the runtime can be any of the supported runtimes, which can be found in the [.NET Core Runtime IDentifier (RID) catalog](https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog).

For example, running the following command in the Search.Tools.Extract folder will result in building in debug mode for the Windows 10 x64 runtime:

`dotnet publish -c debug -r win10-x64` 

## Usage

After publishing, navigate to the output directory and you can run the program. For example the real estate schema file is generated with this command:
`realestate-sample listings <listingsAdminKey> -g histogram.json -h histogram.json  -af description -ak <TextAnalyticsKey> -o RealEstate.json -a city,region,type,status,district -dc price -dn price -dk description`
Once you have generated the schema file, you can optionally edit it by hand and then use the Search.Tools.Generate program to create a LUIS model.

If no parameters are specified, the program will display detailed help as shown below:

`Search.Tools.Extract <Service name> <Index name> <Admin key> [-a <attributeList>] [-ad <domain>] [-af <fieldList>] [-ak <key>] [-al <language>] [-c <file>] [-dc <Field>] [-dk <FieldList>] [-dn <Field>] [-e <examples>] [-f <facetList>] [-g <path>] [-h <path>] [-j <jsonFile>] [-kf <fieldList>] [-km <max>] [-kt <threshold>] [-mo <min>] [-mt <max>] [-o <outputPath>] [-v <field>]
Generate <parameters.IndexName>.json schema file.
If you generate a histogram using -g and -h, it will be used to determine attributes if less than -u unique values are found.
You can find keywords either through -kf for actual keywords or -af to generate keywords using the text analytics cognitive service.
-a <attributeList> : Comma seperated list of field names to generate attributes from.
    If not specified, all string or string[] will be considered for atttributes.
-ad <domain>: Analyze text domain, westus.api.cognitive.microsoft.com by default.
-af <fieldList> : Comma seperated fields to analyze for keywords.
-ak <key> : Key for calling analyze text cognitive service.
-al <language> : Language to use for keyword analysis, en by default.
-c <file> : Copy search index to local JSON file that can be used via -j instead of talking to Azure Search service.
-dc <Field> : Default currency field.
-dn <Field> : Default numeric field.
-dk <FieldList> : Comma seperated list of fields to search for user keywords.
-e <examples> : Number of most frequent examples to keep, default is 3.
-f <facetList>: Comma seperated list of facet names for histogram.  By default all fields in schema.
-g <path>: Generate a file with histogram information from index.  This can take a long time.
-h <path>: Use histogram to help generate schema.  This can be the just generated histogram.
-j <file> : Apply analysis to JSON file rather than search index.
-kf <fieldList> : Comma seperated fields that contain keywords.
-km <max> : Maximum number of keywords to extract, default is 10,000.
-kt <threshold> : Minimum number of docs required to be a keyword, default is 5.
-mo <min> : Minimum number of occurrences for a value to be an attribute candidate, default is 3.
-mt <max> : Maximum number of attributes to allow, default is 5000 and must be < 20,000.
-o <path>: Where to put generated schema.
-s <samples>: Maximum number of rows to sample from index when doing -g.  All by default.
-v <field>: Field to order by when using -g.  There must be no more than 100,000 rows with the same value.  Will use key field if sortable and filterable.
-w : Create a new index from -j JSON file.
{} can be used to comment out arguments.`

## Resources

### .NET Core
[.NET Core Getting Started](https://docs.microsoft.com/en-us/dotnet/articles/core/tutorials/using-with-xplat-cli)
[.NET Core Deployment](https://docs.microsoft.com/en-us/dotnet/articles/core/deploying/index)
[.NET Core Runtime IDentifier (RID) catalog](https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog).

### Azure Search

[Azure Search](https://azure.microsoft.com/en-us/services/search/) 

### LUIS

[LUIS](https://www.luis.ai/) 