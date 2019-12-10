# SimpleRootBot (**DRAFT**)

Bot Framework v4 Root bot for Skills sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a root bot that can consume a remote echo skill.

## Key concepts

- A [root bot](Bots/RootBot.cs) that calls the echo skill and keeps the conversation active until the user says "end" or "stop".
- How to handle and EndOfConversation received from the skill.
- A simple [SkillConversationIdFactory](SkillConversationIdFactory.cs) based on an in memory ConcurrentDictionary used to create and maintain conversation IDs to interact with a skill.
- A [SkillsConfiguration](SkillsConfiguration.cs) class that can load skill definitions from appsettings.
- A [startup](Startup.cs) class that shows how to register the different skills components for DI.
- A [SkillController](Controllers/SkillController.cs) that handles skill responses.
