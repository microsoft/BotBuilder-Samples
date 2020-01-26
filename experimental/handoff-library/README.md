# Handoff Library

Bot Framework v4 Handoff Library

The library implements the additions to the Bot Framework SDK to support handoff to an agent (a.k.a. 'escalation'). The library contains definitons of three event types for signaling handoff operations. Integrations with specific agent hubs is not part of the library. 

The library is intended to be merged in a future release of the Bot Framework SDK.

## Introduction

Successful customer care systems combine conversational AI with live agents. Users want to talk to a live agent either because the bot does not understand the user (AI limitation), or the request cannot be automated and requires a human (application-specific limitation).

The goal of this library is not to offer a universal solution for integration with any customer care system, but rather to provide a "common language" and best practices for bot developers and system integrators building conversational AI systems with human in the loop.

## Protocol

The protocol is centered around three distinct events for initiation, acknowledgement, and completion.

### HandoffInitiation

`HandoffInitiation` event is created by bot to initiate handoff. The event contains the payload as described below.

#### Value

`Value` is an object containing agent hub-specific JSON content, such as required agent skill etc. Example: 
```C#
{ Skill = "credit cards" }
```

#### Attachments

`Attachments` is a list of `Attachment` objects. Bot Framework defines the "Transcript" attachment type that is used to send conversation transcript to the agent hub if required. Attachments can be sent either inline (subject to a size limit) or offline by providing `ContentUrl`. Example:
```C#
handoffEvent.Attachments = new List<Attachment> {
    new Attachment {
        Content = transcript,
        ContentType = "application/json",
        Name = "Trasnscript",
    }};
```    

#### Conversation

The `Conversation` field is an object of type `ConversationAccount` describing the conversation being handed over. Critically, this includes conversation ID that can be used for correlation with other events.

### HandoffResponse

TBD

### HandoffCompleted

TBD

## Example

TBD
