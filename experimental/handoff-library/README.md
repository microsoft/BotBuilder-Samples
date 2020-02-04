# Handoff Library

Bot Framework v4 Handoff Library

The library implements the additions to the Bot Framework SDK to support handoff to an agent (a.k.a. 'escalation'). The library contains definitons of three event types for signaling handoff operations. The events are exchanged between a bot and an _agent hub_ (sometimes also known as engagement hub), which is defined as an application or a system that allows agents (typically human) to receive and handle requests from users, as well as escalation requests from bots.

Integrations with specific agent hubs are not part of the library. 

The library is intto be merged in a future release of the Bot Framework SDK.

## Introduction

Many customer care systems combine conversational AI with live agents. Users want to talk to a live agent either because the bot does not understand the user (AI limitation), or the request cannot be automated and requires a human (application-specific limitation).

The goal of this library is not to offer a universal solution for integration with any customer care system, but rather to provide a "common language" and best practices for bot developers and system integrators building conversational AI systems with human in the loop.

## Protocol

The protocol is centered around three distinct events for initiation, acknowledgement, and completion.

### Handoff Initiation

_Handoff Initiation_ event is created by the bot to initiate handoff. The event contains the payload as described below.

#### Name

The `name` is a REQUIRED field that is set to `"handoff.initiate"`.

#### Value

The `value` field is an object containing agent hub-specific JSON content, such as required agent skill etc. Example: 
```json
{ "Skill" : "credit cards" }
```
`Value` field is OPTIONAL.

#### Attachments

The `attachments` is an OPTIONAL field containing the list of `Attachment` objects. Bot Framework defines the "Transcript" attachment type that is used to send conversation transcript to the agent hub if required. Attachments can be sent either inline (subject to a size limit) or offline by providing `ContentUrl`. Example:
```C#
handoffEvent.Attachments = new List<Attachment> {
    new Attachment {
        Content = transcript,
        ContentType = "application/json",
        Name = "Trasnscript",
    }};
```

Agent hubs SHOULD ignore attachment types they don't understand.

#### Conversation

The `conversation` is a REQUIRED field of type `ConversationAccount` describing the conversation being handed over. Critically, it MUST include the conversation `Id` that can be used for correlation with the other events.

### Handoff Status

_Handoff Status_ event is sent to the bot by the agent hub. The event informs the bot about the status of the initiated handoff operation.

Bots are NOT REQUIRED to handle the event, however they MUST NOT reject it.

#### Name

The `name` is a REQUIRED field that is set to `"handoff.status"`. 

#### Value

The `value` is a REQUIRED field describing the current status of the handoff operation. 
It is a JSON object containing the REQUIRED field `state` and an optional field `message`, as defined below.

The `state` has one of the following values:

- "accepted": An agent has accepted the request and taken control of the conversation.
- "failed": Handoff request has failed. The `message` might contain additional information relevant to the failure.
- "completed": Handoff request has completed.

The format and possible valued of the `message` field are unspecified.

#### Example

Successful handoff completion:

```json
{ "state" : "accepted" }
```

Handoff operation failed due to a timeout:

```json
{ "state" : "failed", "message" : "Cannot find agent with requested skill" }
```

#### Conversation

`Conversation`is a REQUIRED field of type `ConversationAccount` describing the conversation that has been accepted or rejected. The `Id` of the conversation MUST be the same as in the HandoffInitiation that initiated the handoff.

## Example

```C#
protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
{
    if (turnContext.Activity.Text.Contains("human"))
    {
        await turnContext.SendActivityAsync($"You have requested transfer to an agent");

        var transcript = GetTranscript(); // defined elsewhere
        var context = new { Skill = "credit cards" };

        var handoffEvent = EventFactory.CreateHandoffInitiation(turnContext, context, new Transcript(transcript));
        await turnContext.SendActivityAsync(handoffEvent);

        await turnContext.SendActivityAsync($"Agent transfer has been initiated");

    }
    else
    {
        // handle other utterances
    }
}
```
