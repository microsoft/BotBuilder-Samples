# DialogSkillBot (**DRAFT**)

Bot Framework v4 Skills with Dialogs sample.

This bot has been created using [Bot Framework](https://dev.botframework.com), it shows how to create a skill bot that 
can perform different tasks based on requests received from a root bot.

## Key concepts

- A sample [IBot](Bots/SkillBot.cs) that shows how to handle and return EndOfConversation based on the status of the dialog in the skill.
- An [ActivityRouterDialog](Bots/ActivityRouterDialog.cs) that handles Events, Messages and Invoke activities coming from a parent and perform different tasks. 
- How to receive and return values in a skill.
- A [sample skill manifest](wwwroot/manifest/dialogchildbot-manifest-1.0.json) that describes what the skill can do.
