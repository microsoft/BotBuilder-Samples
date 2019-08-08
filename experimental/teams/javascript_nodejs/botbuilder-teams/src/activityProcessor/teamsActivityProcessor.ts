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

import { TurnContext, ActivityTypes } from 'botbuilder';
import { TeamsChannelData, FileDownloadInfo, FileDownloadInfoAttachment } from '../schema';
import { TeamEventType, IMembersAddedEvent, IMembersRemovedEvent, IChannelCreatedEvent, IChannelDeletedEvent, IChannelRenamedEvent, ITeamRenamedEvent } from './teamsEvents';
import { ITeamsInvokeActivityHandler, InvokeActivity } from './teamsInvoke';
import { TeamsFactory } from './teamsFactory';

/**
 * Event handlers for message activities
 */
export interface IMessageActivityHandler {
  /**
   * Handles the message activity.
   * @param context The turn context.
   */
  onMessage?: (context: TurnContext) => Promise<void>;

  /**
   * Handles the message activity with file download info where the file is sent from users to bots.
   * @param context The turn context.
   * @param attachment File download information. Bot can access `downloadUrl` to download the file that user sends to bots.
   */
  onMessageWithFileDownloadInfo?: (context: TurnContext, attachment: FileDownloadInfo) => Promise<void>;
}

/**
 * Event handlers for Teams conversation update activities.
 */
export interface ITeamsConversationUpdateActivityHandler {
  /** 
   * Handles the team members added event. 
   * @param event Event object.
   */
  onTeamMembersAdded?: (event: IMembersAddedEvent) => Promise<void>;

  /** 
   * Handles the team members removed event. 
   * @param event Event object.
   */
  onTeamMembersRemoved?: (event: IMembersRemovedEvent) => Promise<void>;

  /** 
   * Handles the channel created event. 
   * @param event Event object.
   */
  onChannelCreated?: (event: IChannelCreatedEvent) => Promise<void>;

  /** 
   * Handles the channel deleted event.
   * @param event Event object.
   */
  onChannelDeleted?: (event: IChannelDeletedEvent) => Promise<void>;

  /** 
   * Handles the channel renamed event. 
   * @param event Event object.
   */
  onChannelRenamed?: (event: IChannelRenamedEvent) => Promise<void>;

  /** 
   * Handles the team renamed event. 
   * @param event Event object.
   */
  onTeamRenamed?: (event: ITeamRenamedEvent) => Promise<void>;

  /** 
   * Handle generic Teams conversation update event. This handler will be hit only when none of above is applied. 
   * @param turnContext Current turn context.
   */
  onConversationUpdateActivity?: (turnContext: TurnContext) => Promise<void>;
}

/**
 * Event handlers for message reaction activities
 */
export interface IMessageReactionActivityHandler {
  /** 
   * Handles the message reaction activity 
   * @param turnContext Current turn context.
   */
  onMessageReaction?: (turnContext: TurnContext) => Promise<void>;
}

/**
 * Extension types of `ActivityTypes` for Teams
 */
export enum ActivityTypesEx {
  InvokeResponse = 'invokeResponse'
}

/**
 * Teams activity processor - the helper class to dispatch the `onTurn` events to corresponding handlers
 * 
 * @remarks
 * This is the helper class to process every `onTurn` events and dispatch the current event 
 * to corresponding handlers. To use this, simply register and set up the handlers in advance
 * and then forward `onTurn` event to `processIncomingActivity`:
 * ```typescript
 * class Bot {
 *   private activitProc = new TeamsActivityProcessor();
 * 
 *   constructor () {
 *     this.activityProc.messageActivityHandler = {
 *       onMessage: async (ctx: TurnContext) => {
 *         // ... do whatever you want ...
 *       };
 *     };
 *   }
 *  
 *   async onTurn(turnContext: TurnContext) {
 *     await this.activityProc.processIncomingActivity(turnContext);
 *   }
 * }
 * ```
 */
export class TeamsActivityProcessor {
  /**
   * Initializes a new instance of the `TeamsActivityProcessor` class
   * @param messageActivityHandler The message activity handler
   * @param conversationUpdateActivityHandler The conversation update activity handler.
   * @param invokeActivityHandler the invoke activity handler.
   * @param messageReactionActivityHandler The message reaction activity handler.
   */
  constructor (
    public messageActivityHandler?: IMessageActivityHandler,
    public conversationUpdateActivityHandler?: ITeamsConversationUpdateActivityHandler,
    public invokeActivityHandler?: ITeamsInvokeActivityHandler,
    public messageReactionActivityHandler?: IMessageReactionActivityHandler
  ) {
  }

  /**
   * Processes the incoming activity.
   * @param turnContext The correct turn context. 
   */
  public async processIncomingActivity (turnContext: TurnContext) {
    switch (turnContext.activity.type) {
      case ActivityTypes.Message:
        {
          await this.processOnMessageActivity(turnContext);
          break;
        }

      case ActivityTypes.ConversationUpdate:
        {
          await this.processTeamsConversationUpdate(turnContext);
          break;            
        }
      
      case ActivityTypes.Invoke:
        {
          const invokeResponse = await InvokeActivity.dispatchHandler(this.invokeActivityHandler, turnContext);
          if (invokeResponse) {
            await turnContext.sendActivity({ value: invokeResponse, type: ActivityTypesEx.InvokeResponse });
          }
          break;
        }

      case ActivityTypes.MessageReaction:
        {
          const handler = this.messageReactionActivityHandler;
          if (handler && handler.onMessageReaction) {
            await handler.onMessageReaction(turnContext);
          }
          break;
        }
    }
  }

  private async processOnMessageActivity (turnContext: TurnContext) {
    const handler = this.messageActivityHandler;
    if (handler) {
      if (handler.onMessageWithFileDownloadInfo) {
        const attachments = turnContext.activity.attachments || [];
        const fileDownload = attachments.map(x => TeamsFactory.isFileDownloadInfoAttachment(x) && x).shift();
        if (fileDownload) {
          return await handler.onMessageWithFileDownloadInfo(turnContext, fileDownload.content);
        }
      }

      if (handler.onMessage) {
        await handler.onMessage(turnContext);
      }
    }
  }

  private async processTeamsConversationUpdate (turnContext: TurnContext) {
    const handler = this.conversationUpdateActivityHandler;
    if (handler) {
      const channelData: TeamsChannelData = turnContext.activity.channelData;
      if (channelData && channelData.eventType) {
        switch (channelData.eventType as TeamEventType) {
          case 'teamMembersAdded':
            handler.onTeamMembersAdded && await handler.onTeamMembersAdded({
              eventType: 'teamMembersAdded',
              turnContext: turnContext,
              team: channelData.team,
              tenant: channelData.tenant,
              membersAdded: turnContext.activity.membersAdded,
            });
            return;

          case 'teamMembersRemoved':
            handler.onTeamMembersRemoved && await handler.onTeamMembersRemoved({
              eventType: 'teamMembersRemoved',
              turnContext: turnContext,
              team: channelData.team,
              tenant: channelData.tenant,
              membersRemoved: turnContext.activity.membersRemoved
            });
            return;

          case 'channelCreated':
            handler.onChannelCreated && await handler.onChannelCreated({
              eventType: 'channelCreated',
              turnContext: turnContext,
              team: channelData.team,
              tenant: channelData.tenant,
              channel: channelData.channel
            });
            return;

          case 'channelDeleted':
            handler.onChannelDeleted && await handler.onChannelDeleted({
              eventType: 'channelDeleted',
              turnContext: turnContext,
              team: channelData.team,
              tenant: channelData.tenant,
              channel: channelData.channel
            });
            return;

          case 'channelRenamed':
            handler.onChannelRenamed && await handler.onChannelRenamed({
              eventType: 'channelRenamed',
              turnContext: turnContext,
              team: channelData.team,
              tenant: channelData.tenant,
              channel: channelData.channel
            });
            return;

          case 'teamRenamed':
            handler.onTeamRenamed && await handler.onTeamRenamed({
              eventType: 'teamRenamed',
              turnContext: turnContext,
              team: channelData.team,
              tenant: channelData.tenant
            });
            return;
        }  
      }

      if (handler.onConversationUpdateActivity) {
        await handler.onConversationUpdateActivity(turnContext);
      }
    }
  }
}