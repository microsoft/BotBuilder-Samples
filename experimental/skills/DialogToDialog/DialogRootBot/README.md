# DialogRootBot

**DRAFT**

Bot Framework v4 Skills with Dialogs sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a root bot that 
can consume a remote skill capabilities using a SkillDialog to manage the conversation state.

## Key concepts

- A root bot that can call different tasks on a skill:
    - Event Tasks
    - Message Tasks
    - Invoke Tasks
- Send an EndOfConversation activity to remotelly let a skill that it needs to end a conversation.
- Implments a ClaimsValidator that allows a parent bot to validate that a response is coming from a skill that is allowed to talk to the parent.
- A sample SkillDialog that can be used to keep track of muliturn interactions with a skill using the dialog stack.
- A Logger Middleware that shows how to handle and log activities coming from a skill
- A SkillConverdationIdFactory based on IStorage to create and maintain conversation IDs to interact with a skill.
- A SkillsConfiguration class that can load skill definitions from appsettings. 
- A startup.cs that shows how to register the different skills components for DI.
