### Overview of the Experimental Folder

The samples within this folder contain **experimental** work.  These samples are meant to provide a way to solicit
feedback on a given design, approach, or technology being considered by the Bot Framework Team.

The samples in this folder **should not** be used in a production environment.  They are not supported and the team is not implying a given approach used in these samples will be integrated into a future version of the Bot Framework SDK.  Instead, we want to provide a way to engage on topics that can help guide our roadmap for future work.

## Goals of this work

The Bot Framework Team is seeking feedback on a specific problem and possible solutions that exists when developing bots using the Bot Framework SDK.

Each sample contains a reference to the backing GitHub issue(s) that frames the problem and a bit about the approach.  Feedback on the sample should be added to the issue used to track the sample.


## Resources

The samples found in this folder are experimental, do not necessarily relate to one another, are stand-alone, and are not meant to be viewed in any particular order.  None of the samples in the **experimental** folder should be used with bots running in a production environment.



## Experimental samples list

Experimental samples are organized per platform.


| Sample Name           | Description                                                                    | .NET CORE   | NodeJS      | .NET Web API | Typescript  |
|-----------------------|--------------------------------------------------------------------------------|-------------|-------------|--------------|-------------|
|multilingual-luis-bot| The sample shows how to use the library through Middleware to support multilingual interaction with bots in general and LUIS bots in particular.                                                                                                 |[View][cs#1] |  |  | [View][ts#1] |
|qnamaker-activelearning-bot| This sample shows how to integrate Active Learning in a QnA Maker bot.                                                                                                 |[View][cs#2] | [View][js#1]|  |  |

[cs#1]: ./csharp_dotnetcore/multilingual-luis-bot

[cs#2]: ./csharp_dotnetcore/qnamaker-activelearning-bot

[wa#2]: ./csharp_webapi/#

[ts#1]: ./javascript_typescript/multilingual-luis-bot

[js#1]: ./samples/javascript_nodejs/qnamaker-activelearning-bot
