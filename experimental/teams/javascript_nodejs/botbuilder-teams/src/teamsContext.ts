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

import * as builder from 'botbuilder';
import { TeamsConnectorClient } from './';
import * as models from './schema/models';

/**
 * The class to represent Teams context
 * 
 * @remarks
 * This needs to be working with `TeamsMiddleware`:
 * ```JavaScript
 * const { TeamsMiddleware } = require('botbuilder-teams');
 * adapter.use(new TeamsMiddleware());
 * ```
 * With properly configured middleware, you may access the teams context in your `onTurn()` by fetching it from 
 * current TurnContext, i.e., in your bot code:
 * ```JavaScript
 * const { TeamsContext } = require('botbuilder-teams');
 * export class Bot {
 *   async onTurn(turnContext) {
 *     const teamsCtx = TeamsContext.from(ctx);
 *   }
 * }
 * ```
 */
export class TeamsContext {
  public static readonly stateKey: symbol = Symbol('TeamsContextCacheKey');

  constructor (
    public readonly turnContext: builder.TurnContext,
    public readonly teamsConnectorClient: TeamsConnectorClient
  ) {
  }

  public static from(turnContext: builder.TurnContext): TeamsContext {
    return turnContext.turnState.get(TeamsContext.stateKey);
  }

  /**
   * Gets the type of event
   */
  public get eventType(): string {
    return this.getTeamsChannelData().eventType;
  }

  /**
   * Gets info about the team in which this activity fired.
   */
  public get team(): models.TeamInfo {
    return this.getTeamsChannelData().team;
  }

  /**
   * Gets info about the channel in which this activity fired.
   */
  public get channel(): models.ChannelInfo {
    return this.getTeamsChannelData().channel;
  }

  /**
   * Gets tenant info for the activity.
   */
  public get tenant(): models.TenantInfo {
    return this.getTeamsChannelData().tenant;
  }

  /**
   * Gets the general channel for a team.
   * @returns Channel data for general channel.
   */
  public getGeneralChannel(): models.ChannelInfo {
    const channelData = this.getTeamsChannelData();
    if (channelData && channelData.team) {
      return {
        id: channelData.team.id
      };
    }
    throw new Error('Failed to process channel data in Activity. ChannelData is missing Team property.');
  }

  /**
   * Gets the Teams channel data associated with the current activity.
   * @returns Teams channel data for current activity.
   */
  public getTeamsChannelData(): models.TeamsChannelData {
    const channelData = this.turnContext.activity.channelData;
    if (!channelData) {
      throw new Error('ChannelData missing Activity');
    } else {
      return channelData as models.TeamsChannelData;
    }
  }

  /**
   * Gets the activity text without mentions.
   * @returns Text without mentions.
   */
  public getActivityTextWithoutMentions(): string {
    const activity = this.turnContext.activity;
    if (activity.entities && activity.entities.length === 0) {
      return activity.text;
    }
    
    const recvBotId = activity.recipient.id;
    const recvBotMentioned = <builder.Mention[]> activity.entities.filter(e =>
      (e.type === 'mention') && (e as builder.Mention).mentioned.id === recvBotId
    );

    if (recvBotMentioned.length === 0) {
      return activity.text;
    }

    let strippedText = activity.text;
    recvBotMentioned.forEach(m => m.text && (strippedText = strippedText.replace(m.text, '')));
    return strippedText.trim();
  }

  /**
   * Gets the conversation parameters for create or get direct conversation.
   * @param user The user to create conversation with.
   * @returns Conversation parameters to get or create direct conversation (1on1) between bot and user.
   */
  public getConversationParametersForCreateOrGetDirectConversation(user: builder.ChannelAccount): builder.ConversationParameters {
    const channelData = {} as  models.TeamsChannelData;
    if (this.tenant && this.tenant.id) {
      channelData.tenant = {
        id: this.tenant.id
      };
    }

    return <builder.ConversationParameters> {
      bot: this.turnContext.activity.recipient,
      channelData,
      members: [user]
    };
  }

  /**
   * Adds the mention text to an existing activity.
   * 
   * @remarks
   * > [TODO] need to resolve schema problems in botframework-connector where Activity only supports Entity but not Mention.
   * > (entity will be dropped out before sending out due to schema checking)
   * 
   * @param activity The activity.
   * @param mentionedEntity The mentioned entity.
   * @param mentionText The mention text. This is how you want to mention the entity.
   * @returns Activity with added mention.
   */
  public static addMentionToText<T extends builder.Activity>(activity: T, mentionedEntity: builder.ChannelAccount, mentionText?: string): T {
    if (!mentionedEntity || !mentionedEntity.id) {
      throw new Error('Mentioned entity and entity ID cannot be null');
    }

    if (!mentionedEntity.name && !mentionText) {
      throw new Error('Either mentioned name or mentionText must have a value');
    }

    if (!!mentionText) {
      mentionedEntity.name = mentionText;
    }

    const mentionEntityText = `<at>${mentionedEntity.name}</at>`;
    activity.text += ` ${mentionEntityText}`;
    activity.entities = [
      ...(activity.entities || []),
      {
        type: 'mention',
        text: mentionEntityText,
        mentioned: mentionedEntity
      } as builder.Mention
    ];

    return activity;
  }

  /**
   * Notifies the user in direct conversation.
   * @param activity The reply activity.
   * @typeparam T Type of message activity.
   * @returns Modified activity.
   */
  public static notifyUser<T extends builder.Activity>(activity: T): T {
    let channelData: models.TeamsChannelData = activity.channelData || {};
    channelData.notification = {
      alert: true
    };
    activity.channelData = channelData;
    return activity;
  }

  /**
   * Type guard to identify if `channelAccount` is the type of `TeamsChannelAccount`
   * @param channelAccount the channel account or free payload
   * @returns true or false. If returns true then `channelAccount` will be auto casting to `TeamsChannelAccount` in `if () {...}` block
   */
  public static isTeamsChannelAccount(channelAccount: builder.ChannelAccount | any[]): channelAccount is models.TeamsChannelAccount {
    const o = channelAccount as models.TeamsChannelAccount;
    return !!o
        &&(!!o.id && !!o.name)
        && (!!o.aadObjectId || !!o.email || !!o.givenName || !!o.surname || !!o.userPrincipalName);
  }

  /**
   * Type guard to identify if `channelAccount` is the array type of `TeamsChannelAccount[]`
   * @param channelAccount the array of the channel account or free array
   * @returns true if all elements are `TeamsChannelAccount`. If returns true then `channelAccount` will be auto casting to `TeamsChannelAccount[]` in `if () {...}` block
   */
  public static isTeamsChannelAccounts(channelAccount: builder.ChannelAccount[] | any[]): channelAccount is models.TeamsChannelAccount[] {
    return Array.isArray(channelAccount) && channelAccount.every(x => this.isTeamsChannelAccount(x));
  }
}
