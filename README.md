## Overview

Bot Builder samples are organized into groups and designed to illustrate task-focused samples in C# and Node.js to help you build great bots!  To use the samples clone our GitHub repository using Git.

    git clone https://github.com/Microsoft/BotBuilder-Samples.git
    cd BotBuilder-Samples

## Core
These examples show the basic techniques needed to build a great bot.

Sample | Description | C# | Node
------------ | ------------- | :-----------: | :-----------:
Send Attachment | A sample bot that passes simple media attachments (images) to a user activity. | [View Sample](/CSharp/core-SendAttachment)[![Deploy to Azure][Deploy Button]][Deploy CSharp/SendAttachment] | [View Sample](/Node/core-SendAttachment)[![Deploy to Azure][Deploy Button]][Deploy Node/SendAttachment]
Receive Attachment | A sample bot that receives attachments sent by the user and downloads them. | [View Sample](/CSharp/core-ReceiveAttachment)[![Deploy to Azure][Deploy Button]][Deploy CSharp/ReceiveAttachment] | [View Sample](/Node/core-ReceiveAttachment)[![Deploy to Azure][Deploy Button]][Deploy Node/ReceiveAttachment]
Create New Conversation | A sample bot that starts a new conversation using a previously stored user address. | [View Sample](/CSharp/core-CreateNewConversation)[![Deploy to Azure][Deploy Button]][Deploy CSharp/CreateNewConversation] | [View Sample](/Node/core-CreateNewConversation)[![Deploy to Azure][Deploy Button]][Deploy Node/CreateNewConversation]
Proactive Messages | Three sample bots that proactively send a message to the user. | [View Samples](/CSharp/core-proactiveMessages) | [View Samples](/Node/core-proactiveMessages)
Get Members of a Conversation | A sample bot that retrieves the conversation's members list and detects when it changes. | [View Sample](/CSharp/core-GetConversationMembers)[![Deploy to Azure][Deploy Button]][Deploy CSharp/GetConversationMembers] | [View Sample](/Node/core-GetConversationMembers)[![Deploy to Azure][Deploy Button]][Deploy Node/GetConversationMembers]
Direct Line | A sample bot and a custom client communicating to each other using the Direct Line API. | [View Sample](/CSharp/core-DirectLine)[![Deploy to Azure][Deploy Button]][Deploy CSharp/DirectLine] | [View Sample](/Node/core-DirectLine)[![Deploy to Azure][Deploy Button]][Deploy Node/DirectLine]
Direct Line (WebSockets) | A sample bot and a custom client communicating to each other using the Direct Line API + WebSockets. | [View Sample](/CSharp/core-DirectLineWebSockets)[![Deploy to Azure][Deploy Button]][Deploy CSharp/DirectLineWebSockets] | [View Sample](/Node/core-DirectLineWebSockets)[![Deploy to Azure][Deploy Button]][Deploy Node/DirectLineWebSockets]
Basic Multi Dialogs | A sample bot showing how to use the dialog stack. | [View Sample](/CSharp/core-BasicMultiDialog)[![Deploy to Azure][Deploy Button]][Deploy CSharp/BasicMultiDialog] | [View Sample](/Node/core-basicMultiDialog)[![Deploy to Azure][Deploy Button]][Deploy Node/BasicMultiDialog]
Multi Dialogs | A sample bot showing different kind of dialogs. | [View Sample](/CSharp/core-MultiDialogs)[![Deploy to Azure][Deploy Button]][Deploy CSharp/MultiDialogs] | [View Sample](/Node/core-MultiDialogs)[![Deploy to Azure][Deploy Button]][Deploy Node/MultiDialogs]
Global Message Handlers | A sample bot showing the usage of Global Message Handlers to handle global commands and manipulate the dialog stack. | [View Sample](/CSharp/core-GlobalMessageHandlers)[![Deploy to Azure][Deploy Button]][Deploy CSharp/GlobalMessageHandlers] | [View Sample](/Node/core-globalMessageHandlers)[![Deploy to Azure][Deploy Button]][Deploy Node/GlobalMessageHandlers]
Simple Task Automation | A sample bot showing how to do simple task automation scenarios. | [View Sample](/CSharp/capability-SimpleTaskAutomation)[![Deploy to Azure][Deploy Button]][Deploy CSharp/SimpleTaskAutomation] | [View Sample](/Node/capability-SimpleTaskAutomation)[![Deploy to Azure][Deploy Button]][Deploy Node/SimpleTaskAutomation]
Progress Dialog | A sample bot that shows how to create a progress dialog that will periodically notify users of the status of a long running task. | | [View Sample](/Node/core-ProgressDialog)[![Deploy to Azure][Deploy Button]][Deploy Node/ProgressDialog]
State API | A stateless sample bot tracking context of a conversation. | [View Sample](/CSharp/core-State)[![Deploy to Azure][Deploy Button]][Deploy CSharp/State] | [View Sample](/Node/core-State)[![Deploy to Azure][Deploy Button]][Deploy Node/State]
Custom State API | A stateless sample bot tracking context of a conversation using a custom storage provider. | [View Sample](/CSharp/core-CustomState)[![Deploy to Azure][Deploy Button]][Deploy CSharp/CustomState] | [View Sample](/Node/core-CustomState)[![Deploy to Azure][Deploy Button]][Deploy Node/CustomState]
ChannelData | A sample bot sending native metadata to Facebook using ChannelData. | [View Sample](/CSharp/core-ChannelData)[![Deploy to Azure][Deploy Button]][Deploy CSharp/ChannelData] | [View Sample](/Node/core-ChannelData)[![Deploy to Azure][Deploy Button]][Deploy Node/ChannelData]
ChannelData (Share Button) | A sample bot sending native metadata to Facebook to display a Share button. | [View Sample](/CSharp/Blog-CustomChannelData)[![Deploy to Azure][Deploy Button]][Deploy CSharp/BlogCustomChannelData] | [View Sample](/Node/blog-customChannelData)[![Deploy to Azure][Deploy Button]][Deploy Node/BlogCustomChannelData]
AppInsights | A sample bot which logs telemetry to an Application Insights instance. | [View Sample](/CSharp/core-AppInsights)[![Deploy to Azure][Deploy Button]][Deploy CSharp/AppInsights] | [View Sample](/Node/core-AppInsights)[![Deploy to Azure][Deploy Button]][Deploy Node/AppInsights]
Middleware Logging | A basic bot that logs incoming and outgoing messages. | [View Sample](/CSharp/core-Middleware)[![Deploy to Azure][Deploy Button]][Deploy CSharp/MiddlewareLogging] | [View Sample](/Node/capability-middlewareLogging)[![Deploy to Azure][Deploy Button]][Deploy Node/MiddlewareLogging]
Bot in Apps | A sample bot showing how to go beyond by becoming embedded into larger applications. | [View Sample](/CSharp/capability-BotInApps) |

## Cards
These examples emphasize the rich card support in Bot Framework.

Sample | Description | C# | Node
------------ | ------------- | :-----------: | :-----------:
Rich Cards | A sample bot to renders several types of cards as attachments. | [View Sample](/CSharp/cards-RichCards)[![Deploy to Azure][Deploy Button]][Deploy CSharp/RichCards] | [View Sample](/Node/cards-RichCards)[![Deploy to Azure][Deploy Button]][Deploy Node/RichCards]
Carousel of Cards | A sample bot that sends multiple rich card attachments in a single message using the Carousel layout. | [View Sample](/CSharp/cards-CarouselCards)[![Deploy to Azure][Deploy Button]][Deploy CSharp/CarouselCards] | [View Sample](/Node/cards-CarouselCards)[![Deploy to Azure][Deploy Button]][Deploy Node/CarouselCards]
Cards as Attachments | A sample bot that renders several types of cards as attachments, while also showing the generated JSON for each one of these cards at the message's payload. | [View Sample](/CSharp/demo-CardsAttachments)[![Deploy to Azure][Deploy Button]][Deploy CSharp/DemoCardAttachements] | 

## Intelligence
Build bots with powerful algorithms using Bing & Microsoft Cognitive Services APIs.

Sample | Description | C# | Node
------------ | ------------- | :-----------: | :-----------:
LUIS | A sample bot using LuisDialog to integrate with a LUIS.ai application. | [View Sample](/CSharp/intelligence-LUIS)[![Deploy to Azure][Deploy Button]][Deploy CSharp/LUIS] | [View Sample](/Node/intelligence-LUIS)[![Deploy to Azure][Deploy Button]][Deploy Node/LUIS]
Image Caption | A sample bot that gets an image caption using Microsoft Cognitive Services Vision API. | [View Sample](/CSharp/intelligence-ImageCaption)[![Deploy to Azure][Deploy Button]][Deploy CSharp/ImageCaption] | [View Sample](/Node/intelligence-ImageCaption)[![Deploy to Azure][Deploy Button]][Deploy Node/ImageCaption]
Speech To Text | A sample bot that gets text from audio using Bing Speech API. | [View Sample](/CSharp/intelligence-SpeechToText)[![Deploy to Azure][Deploy Button]][Deploy CSharp/SpeechToText] | [View Sample](/Node/intelligence-SpeechToText)[![Deploy to Azure][Deploy Button]][Deploy Node/SpeechToText]
Similar Products | A sample bot that finds visually similar products using Bing Image Search API. | [View Sample](/CSharp/intelligence-SimilarProducts)[![Deploy to Azure][Deploy Button]][Deploy CSharp/SimilarProducts] | [View Sample](/Node/intelligence-SimilarProducts)[![Deploy to Azure][Deploy Button]][Deploy Node/SimilarProducts]
Zummer | A sample bot that finds wikipedia articles using Bing Search API  | [View Sample](/CSharp/intelligence-Zummer)[![Deploy to Azure][Deploy Button]][Deploy CSharp/Zummer] | [View Sample](/Node/intelligence-Zummer)[![Deploy to Azure][Deploy Button]][Deploy Node/Zummer]

## Demo
These are bots designed to showcase end-to-end sample scenarios. They're great sources of code fragments if you're looking to have your bot lightup more complex features.

Sample | Description | C# | Node
------------ | ------------- | :-----------: | :-----------:
Contoso Flowers | A reference implementation using many features from BotFramework. | [View Sample](/CSharp/demo-ContosoFlowers)[![Deploy to Azure][Deploy Button]][Deploy CSharp/ContosoFlowers] | [View Sample](/Node/demo-ContosoFlowers)[![Deploy to Azure][Deploy Button]][Deploy Node/ContosoFlowers]
Azure Search | Two sample bots that help the user navigate large amounts of content. | [View Samples](/CSharp/demo-Search) | [View Samples](/Node/demo-Search)[![Deploy to Azure][Deploy Button]][Deploy Node/Search]
Knowledge Bot | A sample that uses Azure Document DB and Azure Search that searches and filters over an underlying dataset. | [View Sample](/CSharp/sample-KnowledgeBot)[![Deploy to Azure][Deploy Button]][Deploy CSharp/KnowledgeBot] | [View Sample](/Node/sample-knowledgeBot)[![Deploy to Azure][Deploy Button]][Deploy Node/KnowledgeBot]
Roller Skill | A simple dice rolling skill/bot that's been optimized for speech enabled channels, like Cortana. | [View Sample](/CSharp/demo-RollerSkill)[![Deploy to Azure][Deploy Button]][Deploy CSharp/RollerSkill] | [View Sample](/Node/demo-RollerSkill)[![Deploy to Azure][Deploy Button]][Deploy Node/RollerSkill]
Payments | A sample bot showing how to integrate with Microsoft Seller Center for payment processing. | [View Sample](/CSharp/sample-payments)[![Deploy to Azure][Deploy Button]][Deploy CSharp/Payments] | [View Sample](/Node/sample-payments)[![Deploy to Azure][Deploy Button]][Deploy Node/Payments]
LUIS Action Binding | A sample that contains a core implementation for doing LUIS Action Binding. | [View Sample](/CSharp/Blog-LUISActionBinding)[![Deploy to Azure][Deploy Button]][Deploy CSharp/LUISActionBinding] | [View Sample](/Node/blog-LUISActionBinding)
Skype Calling | A sample bot showing how to use the Skype Bot Plaform for Calling API for receiving and handling Skype voice calls. | [View Sample](/CSharp/skype-CallingBot)[![Deploy to Azure][Deploy Button]][Deploy CSharp/SkypeCallingBot] |

[Deploy Button]: https://azuredeploy.net/deploybutton.png
[Deploy CSharp/SendAttachment]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/core-SendAttachment
[Deploy Node/SendAttachment]: https://azuredeploy.net/?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/core-SendAttachment
[Deploy CSharp/ReceiveAttachment]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/core-ReceiveAttachment
[Deploy Node/ReceiveAttachment]: https://azuredeploy.net/?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/core-ReceiveAttachment
[Deploy CSharp/RichCards]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/cards-RichCards
[Deploy Node/RichCards]: https://azuredeploy.net/?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/cards-RichCards
[Deploy CSharp/CarouselCards]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/cards-CarouselCards
[Deploy Node/CarouselCards]: https://azuredeploy.net/?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/cards-CarouselCards
[Deploy CSharp/CreateNewConversation]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/core-CreateNewConversation
[Deploy Node/CreateNewConversation]: https://azuredeploy.net/?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/core-CreateNewConversation
[Deploy CSharp/GetConversationMembers]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/core-GetConversationMembers
[Deploy Node/GetConversationMembers]: https://azuredeploy.net/?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/core-GetConversationMembers
[Deploy CSharp/DirectLine]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/core-DirectLine
[Deploy Node/DirectLine]: https://azuredeploy.net/?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/core-DirectLine
[Deploy CSharp/DirectLineWebSockets]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/core-DirectLineWebSockets
[Deploy Node/DirectLineWebSockets]: https://azuredeploy.net/?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/core-DirectLineWebSockets
[Deploy CSharp/MultiDialogs]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/core-MultiDialogs
[Deploy Node/MultiDialogs]: https://azuredeploy.net/?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/core-MultiDialogs
[Deploy CSharp/State]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/core-State
[Deploy Node/State]: https://azuredeploy.net/?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/core-State
[Deploy CSharp/CustomState]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/core-CustomState
[Deploy Node/CustomState]: https://azuredeploy.net/?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/core-CustomState
[Deploy CSharp/LUIS]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/intelligence-LUIS
[Deploy Node/LUIS]: https://azuredeploy.net/?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/intelligence-LUIS
[Deploy CSharp/ChannelData]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/core-ChannelData
[Deploy Node/ChannelData]: https://azuredeploy.net/?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/core-ChannelData
[Deploy Node/BlogCustomChannelData]: https://azuredeploy.net/?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/blog-customChannelData
[Deploy CSharp/ContosoFlowers]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/demo-ContosoFlowers
[Deploy Node/ContosoFlowers]: https://azuredeploy.net/?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/demo-ContosoFlowers
[Deploy CSharp/ImageCaption]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/intelligence-ImageCaption
[Deploy Node/ImageCaption]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/intelligence-ImageCaption
[Deploy CSharp/SpeechToText]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/intelligence-SpeechToText
[Deploy Node/SpeechToText]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/intelligence-SpeechToText
[Deploy CSharp/SimilarProducts]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/intelligence-SimilarProducts
[Deploy Node/SimilarProducts]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/intelligence-SimilarProducts
[Deploy CSharp/AppInsights]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/core-AppInsights
[Deploy Node/AppInsights]: https://azuredeploy.net/?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/core-AppInsights
[Deploy CSharp/Zummer]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/intelligence-Zummer
[Deploy Node/Zummer]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/intelligence-Zummer
[Deploy Node/ProgressDialog]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/core-ProgressDialog
[Deploy Node/RollerSkill]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/demo-RollerSkill
[Deploy Node/Search]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/demo-Search
[Deploy Node/MiddlewareLogging]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/capability-middlewareLogging
[Deploy Node/SimpleTaskAutomation]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/capability-SimpleTaskAutomation
[Deploy Node/BasicMultiDialog]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/core-basicMultiDialog
[Deploy Node/GlobalMessageHandlers]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/core-globalMessageHandlers
[Deploy Node/KnowledgeBot]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/sample-knowledgeBot
[Deploy Node/Payments]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/Node/sample-payments
[Deploy CSharp/BasicMultiDialog]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/core-BasicMultiDialog
[Deploy CSharp/GlobalMessageHandlers]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/core-GlobalMessageHandlers
[Deploy CSharp/SimpleTaskAutomation]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/capability-SimpleTaskAutomation
[Deploy CSharp/BlogCustomChannelData]: https://azuredeploy.net/?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/Blog-CustomChannelData
[Deploy CSharp/MiddlewareLogging]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/core-Middleware
[Deploy CSharp/DemoCardAttachements]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/demo-CardsAttachments
[Deploy CSharp/KnowledgeBot]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/sample-KnowledgeBot
[Deploy CSharp/RollerSkill]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/demo-RollerSkill
[Deploy CSharp/Payments]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/sample-payments
[Deploy CSharp/LUISActionBinding]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/Blog-LUISActionBinding
[Deploy CSharp/SkypeCallingBot]: https://azuredeploy.net?repository=https://github.com/microsoft/BotBuilder-Samples/tree/master/CSharp/skype-CallingBot