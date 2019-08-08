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
import { MicrosoftAppCredentials } from 'botframework-connector';
import { TeamsConnectorClient, TeamsContext } from '../';

/**
 * Teams Middleware. This middleware needs to be registered in the Middleware pipeline at the time of Adapter initialization.
 */
export class TeamsMiddleware implements builder.Middleware {

  /**
   * When implemented in middleware, process an incoming activity.
   * 
   * @remarks
   * Middleware calls the `next` delegate to pass control to
   * the next middleware in the pipeline. If middleware doesn’t call the next delegate,
   * the adapter does not call any of the subsequent middleware’s request handlers or the
   * bot’s receive handler, and the pipeline short circuits.
   * The `context` provides information about the
   * incoming activity, and other data needed to process the activity.
   * 
   * @param context The context object for this turn.
   * @param next The delegate to call to continue the bot middleware pipeline.
   */
  public async onTurn(context: builder.TurnContext, next: () => Promise<void>): Promise<void> {
    const credentials: MicrosoftAppCredentials = context.adapter && (context.adapter as any).credentials;
    if (context.activity.channelId === 'msteams' && !!credentials) {
      const serviceUrl = context.activity && context.activity.serviceUrl;
      const teamsConnectorClient = new TeamsConnectorClient(credentials, { baseUri: serviceUrl });
      context.turnState.set(TeamsContext.stateKey, new TeamsContext(context, teamsConnectorClient));
    }
    return next();
  }
}
