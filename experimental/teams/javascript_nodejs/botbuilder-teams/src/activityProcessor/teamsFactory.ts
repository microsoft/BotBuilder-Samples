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

import { CardFactory, Attachment, CardAction } from 'botbuilder';
import * as models from '../schema';
import * as ac from 'adaptivecards';

export type IAdaptiveCardAction = ac.IAdaptiveCard['actions'][0];

export interface ITaskModuleCardAction {
  title: string;
  value: {[key: string]: any};
  toAction(): CardAction;
  toAdaptiveCardAction(): IAdaptiveCardAction;
}

/**
 * Teams factory class that extends `CardFacotry` to create Teams-specific cards and types
 */
export class TeamsFactory extends CardFactory {
  /**
   * List of content types for each Teams-specific card style.
   */
  public static contentTypes: any = {
    ...CardFactory.contentTypes,
    o365ConnectorCard: 'application/vnd.microsoft.teams.card.o365connector',
    fileConsentCard: 'application/vnd.microsoft.teams.card.file.consent',
    fileDownloadInfo: 'application/vnd.microsoft.teams.file.download.info',
    fileInfoCard: 'application/vnd.microsoft.teams.card.file.info'
  };

  /**
   * Type guard to identify this `attachment` is the type of `FileDownloadInfoAttachment`
   * @param attachment Generic attachment object.
   * @returns Returns true if `attachment` is type of `FileDownloadInfoAttachment` 
   * (and will auto cast to this type in `if() {}` block)
   */
  public static isFileDownloadInfoAttachment (attachment: Attachment): attachment is models.FileDownloadInfoAttachment {
    return attachment.contentType === TeamsFactory.contentTypes.fileDownloadInfo;
  }

  /**
   * Returns an attachment for an O365 connector card.
   * @param content card payload
   */
  public static o365ConnectorCard (content: models.O365ConnectorCard): Attachment {
    return {
      contentType: TeamsFactory.contentTypes.o365ConnectorCard,
      content
    };
  }

  /**
   * Returns an attachment for an file consent card
   * 
   * @remarks
   * The file consent card is used to send user a card to ask if he/she would like to receive the file
   * before bot sends out the file. If user accepts it then the `onFileConsent` invoke handler will be 
   * triggered (defined in `ActivityProcessor.invokeActivityHandler`) where an upload URL comes with the
   * invoke request. Therefore bots can upload content there.
   * 
   * @param fileName file name
   * @param content card payload
   */
  public static fileConsentCard (fileName: string, content: models.FileConsentCard): Attachment {
    return {
      contentType: TeamsFactory.contentTypes.fileConsentCard,
      name: fileName,
      content
    };
  }

  /**
   * Returns an attachment for an file info card
   * 
   * @remarks
   * After user accepts consents and bot uploads file to the URL endpoint, then bot can send out this
   * type of card to inform user that the file is ready to download.
   * 
   * @param fileName file name
   * @param contentUrl the content URL to notify user to download the file
   * @param content card payload
   */
  public static fileInfoCard (fileName: string, contentUrl: string, content: models.FileInfoCard): Attachment {
    return {
      contentType: TeamsFactory.contentTypes.fileInfoCard,
      name: fileName,
      contentUrl,
      content
    };
  }

  public static adaptiveCard(card: ac.IAdaptiveCard): Attachment {
    return CardFactory.adaptiveCard(card);
  }

  public static adaptiveCardAction(action: CardAction | string | undefined): IAdaptiveCardAction {
    const obj = TeamsFactory.adaptiveCardActions([action]);
    return (obj && obj[0]) || undefined;
  }

  public static adaptiveCardActions(actions: (CardAction | string)[] | undefined): IAdaptiveCardAction[] {
    let botBuilderBtns = CardFactory.actions(actions);
    let acActions: IAdaptiveCardAction[] = [];
    for (let i = 0; i < botBuilderBtns.length; ++i) {
      let btn = botBuilderBtns[i];
      let adapterBtn: IAdaptiveCardAction = {
        id: undefined,
        type: 'Action.Submit',
        title: btn.title,
        data: {},
      };
      delete btn.title;
      adapterBtn.data[ 'msteams' ] = btn;
      acActions.push(adapterBtn);
    }
    return acActions;
  }

  public static taskModuleAction(title: string, value: {[key: string]: any} = {}): ITaskModuleCardAction {
    let adaptorObj: CardAction = {
      type: 'invoke',
      title,
      value
    };

    let toAction = (): CardAction => {
      let valJson = (typeof adaptorObj.value === 'string') ? JSON.parse(adaptorObj.value) : adaptorObj.value;
      valJson.type = 'task/fetch';
      adaptorObj.value = JSON.stringify(valJson);
      return adaptorObj;
    };

    let toAdaptiveCardAction = (): IAdaptiveCardAction => {
      let btn = toAction();
      return TeamsFactory.adaptiveCardAction(btn);
    };

    return {
      ...adaptorObj,
      toAction,
      toAdaptiveCardAction
    };
  }
}
