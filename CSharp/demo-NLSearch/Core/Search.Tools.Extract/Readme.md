# Search Powered Bots Tools: Extract

The Extract tool generates a histogram and computes different attributes and properties of an Azure Search index. The output of this program is then passed to the Generate program, to jointly generate a LUIS model for Search Powered Bots.

## Build

To build this tool, the command below needs to be executed after specifying the mode and runtime parameters.

`dotnet publish -c <mode> -r <runtime>`

The mode can be either *debug* or *release*, and the runtime can be any of the supported runtimes, which can be found in the [.NET Core Runtime IDentifier (RID) catalog](https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog).

For example, running the following command in the Search.Tools.Extract folder will result in building in debug mode for the Windows 10 x64 runtime:

`dotnet publish -c debug -r win10-x64` 

## Publish

Even if Search.Tools.Extract is a console application, building it does not generate an executable, because this is a .NET Core console application. To obtain an executable file, the project needs to be published.

`dotnet publish -c <mode> -r <runtime>`

The mode can be either *debug* or *release*, and the runtime can be any of the supported runtimes, which can be found in the [.NET Core Runtime IDentifier (RID) catalog](https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog).

A valid publish example command could be:

`dotnet publish -c release -r debian.8-x64`

Which would output a release mode executable for Debian 8.

## Usage

After publishing, navigate to the output directory and you can run the program. If no parameters are specified, the program will display detailed help and information about the parameters accepted.

The form of a call to Search.Tools.Extract is as follows:

`Search.Tools.Extract <Azure Search Index> <Azure Search Service Key -g <.bin file name for histogram> -v <order by field when using -g> -h <.bin file for histogram> -o <json output location>` 

And here is an example call:

`Search.Tools.Extract realestate listings 93B04DA93FF693841A35B66AF9D32023 -g RealEstateBot-histogram.bin -v price -h RealEstateBot-histogram.bin -o RealEstateBot.json`

## Resources

### .NET Core
[.NET Core Getting Started](https://docs.microsoft.com/en-us/dotnet/articles/core/tutorials/using-with-xplat-cli)
[.NET Core Deployment](https://docs.microsoft.com/en-us/dotnet/articles/core/deploying/index)
[.NET Core Runtime IDentifier (RID) catalog](https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog).

### Azure Search

[Azure Search](https://azure.microsoft.com/en-us/services/search/) 

### LUIS

[LUIS](https://www.luis.ai/) 