This sample shows code to manipulate TIMEX expressions.
# Concepts introduced in this sample
## What is a TIMEX expression?
A TIMEX expression is an alpha-numeric expression derived in outline from the standard date-time representation ISO 8601.
The interesting thing about TIMEX expressions is that they can represent various degrees of ambiguity in the date parts. For example, May 29th, is not a
full calendar date because we have said which May 29th - it could be this year, last year, any year in fact.
TIMEX has other features such as teh ability to represent ranges, date ranges, time ranges and even date-time ranges.

# To try this sample
- Clone the repository
```bash
git clone https://github.com/microsoft/botbuilder-samples.git
```
# Prerequisites
## Visual studio
- Navigate to the samples folder (`botbuilder-samples/samples/csharp_dotnetcore/40.timex-resolution`) and open Timex-Resolution.csproj in Visual studio 
- Run the project (press `F5` key)

## Visual studio code
- Open `botbuilder-samples/samples/csharp_dotnetcore/40.timex-resolution` folder
- Bring up a terminal, navigate to `botbuilder-samples/samples/csharp_dotnetcore/01.console-echo` folder
- Type `dotnet run`.

## Update packages
- In Visual Studio right click on the solution and select "Restore NuGet Packages".
  **Note:** this sample requires `Microsoft.Recognizers.Text.DataTypes.TimexExpression` and `Microsoft.Recognizers.Text.DateTime`.
- In Visual Studio Code type `dotnet restore`

# Further reading
- [Azure Bot Service Introduction](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Bot basics](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Channels and Bot Connector service](https://docs.microsoft.com/en-us/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)
- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [TIMEX] (https://en.wikipedia.org/wiki/TimeML#TIMEX3)
- [ISO 8601] (https://en.wikipedia.org/wiki/ISO_8601)
