# DialogRootBot (**DRAFT**)

Bot Framework v4 Skills with Dialogs sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a root bot that 
can consume a remote skill capabilities using a SkillDialog to manage the conversation state.

## Key concepts

- A [root dialog](Dialogs/MainDialog.cs) that can call different tasks on a skill using a [SkillDialog](Dialogs/SkillDialog.cs):
    - Event Tasks
    - Message Tasks
    - Invoke Tasks
- How to send an EndOfConversation activity to remotely let a skill that it needs to end a conversation.
- How to Implement a [ClaimsValidator](Authentication/AllowedCallersClaimsValidator.cs) that allows a parent bot to validate that a response is coming from a skill that is allowed to talk to the parent.
- A sample [SkillDialog](Dialogs/SkillDialog.cs) that can be used to keep track of multiturn interactions with a skill using the dialog stack.
- A [Logger Middleware](Middleware/LoggerMiddleware.cs) that shows how to handle and log activities coming from a skill
- A [SkillConversationIdFactory](SkillConversationIdFactory.cs) based on IStorage to create and maintain conversation IDs to interact with a skill.
- A [SkillsConfiguration](SkillsConfiguration.cs) class that can load skill definitions from appsettings.
- A [startup](Startup.cs) class that shows how to register the different skills components for DI.
- A [SkillController](Controllers/SkillController.cs) that handles skill responses.
