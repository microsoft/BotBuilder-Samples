# Handoff Library

Bot Framework v4 Handoff Library

The library implements the additions to the Bot Framework SDK to support handoff to an agent (a.k.a. 'escalation').

The library is intended to be merged in a future release of the Bot Framework SDK.

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## Handoff APIs

The developer experience consists of two parts – the initiation of the handoff, which is an API call triggered by the bot, and the completion notification, which is an event received by the bot.

### Adapter Support

A bot that supports handoff must use a bot adapter implementing the `IHandoffAdapter` interface, defined as follows:

```
public interface IHandoffAdapter
{
    Task<IHandoffRequest> InitiateHandoffAsync(Activity[] activities, 
                                               object handoffContext, 
                                               CancellationToken cancellationToken = default);
}
```

The payload of the call contains the transcript of the activities and the channel specific context.

The `BotFrameworkHttpAdapterWithHandoff` supports handoff by implementing the `IHandoffAdapter` interface. The payload is packaged into an HTTP POST request and is sent to the channel. Other implementations are possible.

### Handoff Initiation

The handoff is initiated by the bot. This can be triggered by NLP ("I need a human"), failure to handle user request, or sentiment analysis ("you are not understanding me").

The handoff is initiated via the `InitiateHandoffAsync` call on the `ITurnContext` interface:

```C#
public class BotWithHandoff : ActivityHandler
{
    public override async Task OnTurnAsync(ITurnContext turnContext, 
        CancellationToken cancellationToken = default)
    {
        ...
        var request = await turnContext.InitiateHandoffAsync(/* omitted, see below */);
    }
}
```

Because the operation can take a long time (several minutes), the returning task does not represent the completion of the handoff. Instead, the API returns the `HandoffRequest` class that can be used to track the completion of the request:

```C#
public class HandoffRequest
{
    public bool IsCompletedAsync();
}
```

Agent hubs can implement this interface providing additional functionality. For example, an agent hub can support querying the length of the customer queue, which is not available for all agent hubs.
Additionally, the handoff completion is signaled by the Handoff event, as shown below.

### Example

```C#
protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
{
    if (turnContext.Activity.Text.Contains("human"))
    {
        await turnContext.SendActivityAsync("You will be transferred to a human agent. Sit tight.");

        var a1 = MessageFactory.Text($"first message");
        var a2 = MessageFactory.Text($"second message");
        var transcript = new Activity[] { a1, a2 };
        var context = new { Skill = "credit cards" };
        var request = await turnContext.InitiateHandoffAsync(transcript, context, cancellationToken);

        if (await request.IsCompletedAsync())
        {
            await turnContext.SendActivityAsync("Handoff request has been completed");
        }
        else
        {
            await turnContext.SendActivityAsync("Handoff request has NOT been completed");
        }
    }
    else
    {
        await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);
    }
}

protected override async Task OnHandoffActivityAsync(ITurnContext<IHandoffActivity> turnContext, CancellationToken cancellationToken)
{
    var conversationId = turnContext.Activity.Conversation.Id;
    await turnContext.SendActivityAsync($"Received Handoff ack for conversation {conversationId}");
}
```

## To try the library

- Clone the repository

    ```bash
    https://github.com/microsoft/BotBuilder-Samples.git
    ```
- Build the solution

- Deploy and run the Handoff bot

- Using the DirectLine API, [start a conversation](https://docs.microsoft.com/en-us/azure/bot-service/rest-api/bot-framework-rest-direct-line-3-0-start-conversation?view=azure-bot-service-4.0).

- [Send a message](https://docs.microsoft.com/en-us/azure/bot-service/rest-api/bot-framework-rest-direct-line-3-0-send-activity?view=azure-bot-service-4.0) to the bot containing the word 'human'

- The bot should receive the handoff activity when the handoff has taken place
