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
import { TeamsChannelData } from '../schema/models';

/**
 * Teams specific conversation state management.
 */
export class TeamSpecificConversationState extends builder.BotState {
  /**
   * Initializes a new instance of the `TeamSpecificConversationState`
   * @param storage The storage provider to use.
   */
  constructor (storage: builder.Storage) {
    super(storage, (turnContext) => this.getStorageKey(turnContext));
  }

  /**
   * Gets the key to use when reading and writing state to and from storage.
   * @param turnContext The context object for this turn.
   * @returns The storage key.
   */
  private getStorageKey (turnContext: builder.TurnContext): Promise<string> {
    const teamsChannelData: TeamsChannelData = turnContext.activity.channelData;
    if (teamsChannelData.team && teamsChannelData.team.id) {
      return Promise.resolve(`team/${turnContext.activity.channelId}/${teamsChannelData.team.id}`);
    } else {
      return Promise.resolve(`chat/${turnContext.activity.channelId}/${turnContext.activity.conversation.id}`);
    }
  }
}