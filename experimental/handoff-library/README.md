# Handoff Library

Bot Framework v4 Handoff Library

The library implements the additions to the Bot Framework SDK to support handoff to an agent (a.k.a. 'escalation'). The library contains definitons of three event types for signaling handoff operations. The events are exchanged between a bot and an _agent hub_ (sometimes also known as engagement hub), which is defined as an application or a system that allows agents (typically human) to receive and handle requests from users, as well as escalation requests from bots.

Integrations with specific agent hubs are not part of the library. 

The library is intended to be merged in a future release of the Bot Framework SDK.

## Introduction

Successful customer care systems combine conversational AI with live agents. Users want to talk to a live agent either because the bot does not understand the user (AI limitation), or the request cannot be automated and requires a human (application-specific limitation).

The goal of this library is not to offer a universal solution for integration with any customer care system, but rather to provide a "common language" and best practices for bot developers and system integrators building conversational AI systems with human in the loop.

## Protocol

The protocol is centered around three distinct events for initiation, acknowledgement, and completion.

### HandoffInitiation

`HandoffInitiation` event is created by the bot to initiate handoff. The event contains the payload as described below.

#### Value

`Value` field is an object containing agent hub-specific JSON content, such as required agent skill etc. Example: 
```C#
{ Skill = "credit cards" }
```
`Value` field SHOULD be present in the event.

#### Attachments

`Attachments` is an OPTIONAL field containing the list of `Attachment` objects. Bot Framework defines the "Transcript" attachment type that is used to send conversation transcript to the agent hub if required. Attachments can be sent either inline (subject to a size limit) or offline by providing `ContentUrl`. Example:
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

The `Conversation` is a REQUIRED field that is an object of type `ConversationAccount` describing the conversation being handed over. Critically, it MUST include the conversation `Id` that can be used for correlation with the other events.

### HandoffResponse

HandoffResponse is an OPTIONAL event sent to the bot when the handoff request has been accepted or rejected by the agent hub. Bots are NOT REQUIRED to handle the event, however they MUST NOT reject it.

#### Value

`Value` is a REQUIRED field indicating whether the handoff request has been accepted. 
Possible values are: 

- "accepted": An agent has accepted the request and taken control of the conversation.
- "failed": Handoff request has failed.
- "timedOut": Handoff request has timed out.

#### Conversation

`Conversation`is a REQUIRED field that is an object of type `ConversationAccount` describing the conversation that has been accepted or rejected. The `Id` of the conversation MUST be the same as in the HandoffInitiation that initiated the handoff.

Other fields are not specified by the protocol, however agent hub implementations MAY include additional error payload in `Attachments`. For example, a "failed" event might contain error reason such as 'agent with requested skill not found'. 

### HandoffCompleted

HandoffCompleted is an OPTIONAL event sent to the bot when the handoff operation has ended. The event indicates that the agent has disconnected and the control has been transferred back to the bot. Bots SHALL NOT rejects this event.

#### Value

`Value` is a code that indicates how the handoff request has completed. Possible values are: 

- "endOfConversation": The conversation has ended successfully (i.e. an agent has handled the conversation). The bot is not expected to continue the conversation.
- "transferBack": The agent has completed its part of the conversation and is transferring the conversatoin back to the bot. The bot is expected to continue the conversation. 
- "failed". The handoff request has been accepted the by the agent but later failed (for example, the agent got disconnected). The bot SHOULD continue processing the user request, or retry the handoff.

#### Attachments

For "transferBack" events, the `Attachments` field MAY contain the transcript of the conversation that took place between the user and the agent. The bot can inspect the transcript and perform additional action. For example, the last record in the transcript can be "I'm transferring you back to the bot to complete your hotel reservation".

#### Conversation

`Conversation` is an object of type `ConversationAccount` describing the conversation that has just completed. The `Id` of the conversation MUST be the same as in the HandoffInitiation that initiated the handoff.

## Example

TBD
