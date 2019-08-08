// Copyright (c) Microsoft Corporation
// All rights reserved.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

import * as models from '../schema/models';
import { TurnContext, ChannelAccount } from 'botbuilder';

/**
 * Literal type of team event name values.
 */
export type TeamEventType =
    'teamMembersAdded' 
  | 'teamMembersRemoved'
  | 'channelCreated'
  | 'channelDeleted'
  | 'channelRenamed'
  | 'teamRenamed';

/**
 * Type of team event.
 */
export interface ITeamEvent
{
  /** Gets the event type. */
  readonly eventType: TeamEventType;

  /** Gets the team for the tenant. */
  readonly team: models.TeamInfo;

  /** Gets the tenant for the team. */
  readonly tenant: models.TenantInfo;

  /** Gets the original activity. */
  readonly turnContext: TurnContext;
}

/**
 * Channel created event arguments.
 */
export interface IChannelCreatedEvent extends ITeamEvent {
  /** Gets the event type - must be `channelCreated` */
  readonly eventType: 'channelCreated';

  /** Gets the created channel. */
  readonly channel: models.ChannelInfo
}

/**
 * Channel deleted event arguments.
 */
export interface IChannelDeletedEvent extends ITeamEvent {
  /** Gets the event type - must be `channelDeleted` */
  readonly eventType: 'channelDeleted';

  /** Gets the deleted channel. */
  readonly channel: models.ChannelInfo;
}

/**
 * Channel renamed event.
 */
export interface IChannelRenamedEvent extends ITeamEvent {
  /** Gets the event type - must be `channelRenamed` */
  readonly eventType: 'channelRenamed';

  /** Gets the renamed channel. */
  readonly channel: models.ChannelInfo;
}

/**
 * Event arguments for members added event.
 */
export interface IMembersAddedEvent extends ITeamEvent {
  /** Gets the event type - must be `teamMembersAdded` */
  readonly eventType: 'teamMembersAdded';

  /** Gets the list of added members. */
  readonly membersAdded: ChannelAccount[];
}

/**
 * Event arguments for members removed event.
 */
export interface IMembersRemovedEvent extends ITeamEvent {
  /** Gets the event type - must be `teamMembersRemoved` */
  readonly eventType: 'teamMembersRemoved';

  /** Gets the list of removed members. */
  readonly membersRemoved: ChannelAccount[];
}

/**
 * Team renamed event.
 */
export interface ITeamRenamedEvent extends ITeamEvent {
  /** Gets the event type - must be `teamRenamed` */
  readonly eventType: 'teamRenamed';
}
