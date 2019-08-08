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

import { BotFrameworkAdapter, ConversationReference, TurnContext, ConversationParameters, Activity, ConversationAccount, ResourceResponse, ActivityTypes } from 'botbuilder';
import { ConnectorClient } from 'botframework-connector';
import { TeamsChannelData, TeamsChannelAccount } from './schema';
import { TeamsContext } from './teamsContext';

/**
 * A Teams bot adapter inheriting from BotFrameworkAdapter that connects your bot to Bot Framework channels.
 *
 * @remarks
 * Similar to BotFrameworkAdapter, Use this adapter to connect your bot to the Bot Framework service.
 * As inheriting from BotFrameworkAdapter, TeamsAdapter not only provides all functionalities of a regular bot adapter,
 * but also provides with additional Teams-binding operations and types.
 * 
 * The following example shows the typical adapter setup:
 *
 * ```JavaScript
 * const { TeamsAdapter } = require('botbuilder-teams');
 *
 * const adapter = new TeamsAdapter({
 *    appId: process.env.MICROSOFT_APP_ID,
 *    appPassword: process.env.MICROSOFT_APP_PASSWORD
 * });
 * ```
 */
export class TeamsAdapter extends BotFrameworkAdapter {
  /**
   * Lists the members of a given activity as specified in a TurnContext.
   *
   * @remarks
   * Returns an array of ChannelAccount objects representing the users involved in a given activity.
   *
   * This is different from `getConversationMembers()` in that it will return only those users
   * directly involved in the activity, not all members of the conversation.
   * @param context Context for the current turn of conversation with the user.
   * @param [activityId] (Optional) activity ID to enumerate. If not specified the current activities ID will be used.
   * @returns an array of TeamsChannelAccount for the given context and/or activityId
   */
  public async getActivityMembers(context: TurnContext, activityId?: string): Promise<TeamsChannelAccount[]> {
    const members = await super.getActivityMembers(context, activityId)
      .then((res: any) => 
        this.merge_objectId_and_aadObjectId(JSON.parse(res._response.bodyAsText)));
    if (TeamsContext.isTeamsChannelAccounts(members)) {
      return members;
    } else {
      throw new Error('Members are not TeamsChannelAccount[]');
    }
  }
  
  /**
   * Lists the members of the current conversation as specified in a TurnContext.
   *
   * @remarks
   * Returns an array of ChannelAccount objects representing the users currently involved in the conversation
   * in which an activity occured.
   *
   * This is different from `getActivityMembers()` in that it will return all
   * members of the conversation, not just those directly involved in the activity.
   * @param context Context for the current turn of conversation with the user.
   * @returns an array of TeamsChannelAccount for the given context
   */
  public async getConversationMembers(context: TurnContext): Promise<TeamsChannelAccount[]> {
    const members = await super.getConversationMembers(context)
      .then((res: any) =>
        this.merge_objectId_and_aadObjectId(JSON.parse(res._response.bodyAsText)));      
    if (TeamsContext.isTeamsChannelAccounts(members)) {
      return members;
    } else {
      throw new Error('Members are not TeamsChannelAccount[]');
    }
  }

  /**
   * Starts a new conversation with a user. This is typically used to Direct Message (DM) a member
   * of a group.
   *
   * @remarks
   * This function creates a new conversation between the bot and a single user, as specified by
   * the ConversationReference passed in. In multi-user chat environments, this typically means
   * starting a 1:1 direct message conversation with a single user. If called on a reference
   * already representing a 1:1 conversation, the new conversation will continue to be 1:1.
   *
   * * In order to use this method, a ConversationReference must first be extracted from an incoming
   * activity. This reference can be stored in a database and used to resume the conversation at a later time.
   * The reference can be created from any incoming activity using `TurnContext.getConversationReference(context.activity)`.
   *
   * The processing steps for this method are very similar to [processActivity()](#processactivity)
   * in that a `TurnContext` will be created which is then routed through the adapters middleware
   * before calling the passed in logic handler. The key difference is that since an activity
   * wasn't actually received from outside, it has to be created by the bot.  The created activity will have its address
   * related fields populated but will have a `context.activity.type === undefined`..
   *
   * ```JavaScript
   * // Get group members conversation reference
   * const reference = TurnContext.getConversationReference(context.activity);
   *
   * // Start a new conversation with the user
   * await adapter.createConversation(reference, async (ctx) => {
   *    await ctx.sendActivity(`Hi (in private)`);
   * });
   * ```
   * @param reference A `ConversationReference` of the user to start a new conversation with.
   * @param logic A function handler that will be called to perform the bot's logic after the the adapters middleware has been run.
   */
  public async createTeamsConversation(reference: Partial<ConversationReference>, tenantIdOrTurnContext: string | TurnContext, logic?: (context: TurnContext) => Promise<void>): Promise<void> {
    if (!reference.serviceUrl) { throw new Error(`TeamsAdapter.createTeamsConversation(): missing serviceUrl.`); }

    // Create conversation
    const tenantId: string = (tenantIdOrTurnContext instanceof TurnContext)
      ? TeamsContext.from(tenantIdOrTurnContext).tenant.id || undefined
      : <string> tenantIdOrTurnContext;
    const channelData: TeamsChannelData = { tenant: {id: tenantId} };
    const parameters: ConversationParameters = { bot: reference.bot, members: [reference.user], channelData } as ConversationParameters;
    const client: ConnectorClient = this.createConnectorClient(reference.serviceUrl);
    const response = await client.conversations.createConversation(parameters);

    // Initialize request and copy over new conversation ID and updated serviceUrl.
    const request: Partial<Activity> = TurnContext.applyConversationReference(
      { type: 'event', name: 'createConversation' },
      reference,
      true
    );
    request.conversation = { id: response.id } as ConversationAccount;
    if (response.serviceUrl) { request.serviceUrl = response.serviceUrl; }

    // Create context and run middleware
    const context: TurnContext = this.createContext(request);
    await this.runMiddleware(context, logic as any);
  }

  /**
   * Create a new reply chain in the current team context.
   *
   * @remarks
   * If a bot receives messages and wants to reply it as a new reply chain rather than reply it in the same thread, 
   * then this method can be used. This method uses `turnContext.activity` as the conversation reference to figure out
   * the channel id in the current context and sends out activities as new messages in this team channel, so new reply
   * chain is created and it won't reply to the original thread. The usages looks like:
   * 
   * ```JavaScript
   * await adapter.createReplyChain(ctx, [
   *   { text: 'New reply chain' }
   * ]);
   * ```
   * @param turnContext current TurnContext
   * @param activities One or more activities to send to the team as new reply chain.
   * @param [inGeneralChannel] (optional) set it true if you want to create new reply chain in the general channel
   */
  public createReplyChain(turnContext: TurnContext, activities: Partial<Activity>[], inGeneralChannel?: boolean): Promise<ResourceResponse[]> {
    let sentNonTraceActivity: boolean = false;
    const teamsCtx = TeamsContext.from(turnContext);
    const ref: Partial<ConversationReference> = TurnContext.getConversationReference(turnContext.activity);
    const output: Partial<Activity>[] = activities.map((a: Partial<Activity>) => {
      const o: Partial<Activity> = TurnContext.applyConversationReference({...a}, ref);
      try {
        o.conversation.id = inGeneralChannel
          ? teamsCtx.getGeneralChannel().id
          : teamsCtx.channel.id;
      } catch (e) {
        // do nothing for fields fetching error
      }
      if (!o.type) { o.type = ActivityTypes.Message; }
      if (o.type !== ActivityTypes.Trace) { sentNonTraceActivity = true; }
      return o;
    });

    return turnContext['emit'](turnContext['_onSendActivities'], output, () => {
      return super.sendActivities(turnContext, output)
        .then((responses: ResourceResponse[]) => {
          // Set responded flag
          if (sentNonTraceActivity) { turnContext.responded = true; }
          return responses;
        });
    });
  }

  /**
   * SMBA sometimes returns `objectId` and sometimes returns `aadObjectId`.
   * Use this function to unify them into `aadObjectId` that is defined by schema.
   * @param members raw members array
   */
  private merge_objectId_and_aadObjectId(members: any[]): any[] {
    if (members) {
      members.forEach(m => {
        if (!m.aadObjectId && m.objectId) {
          m.aadObjectId = m.objectId;
        }
        delete m.objectId;
      });
    }
    return members;
  }
}