# Handoff Library

Bot Framework v4 Handoff Library

The library implements the additions to the Bot Framework SDK to support handoff to an agent (a.k.a. 'escalation').

The library is intended to be merged in a future release of the Bot Framework SDK.

## Introduction

TBD

## Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download) version 2.1

  ```bash
  # determine dotnet version
  dotnet --version
  ```

## Protocol

The protocol is centered around three distinct events for initiation, acknowledgement, and completion.

### `HandoffInitiation`

TBD

#### Purpose

This event is used by bot to initiate handoff.

### Payload

`Value`: an object containing agent hub-specific JSON content, such as required agent skill etc. Example: 
```C#
{ Skill = "credit cards" }
```
`Attachments`: a list of Attachment objects. Bot Framework defines the "Transcript" attachment type that is used to send conversation transcript to the agent hub if required. Attachments can be sent either inline (subject to a size limit) or offline by providing `ContentUrl`. Example:
```C#
handoffEvent.Attachments = new List<Attachment> {
    new Attachment {
        Content = activities,
        ContentType = "application/json",
        Name = "Trasnscript",
    }};
```    
`Conversation`: an object of type ConversationAccount describing the conversation being handed over. Critically, this includes conversation ID that can be used for correlation with other events.

### `HandoffResponse`

TBD

### `HandoffCompleted`

TBD

## Example

TBD
