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
import * as teams from '../models';

/**
 * Defines values for Type.
 * Possible values include: 'ViewAction', 'OpenUri', 'HttpPOST', 'ActionCard'
 * @readonly
 * @enum {string}
 */
export type O365ConnectorCardActionType = 'ViewAction' | 'OpenUri' | 'HttpPOST' | 'ActionCard';

/**
 * @interface
 * An interface representing O365ConnectorCardActionBase.
 * O365 connector card action base
 *
 */
export interface O365ConnectorCardActionBase {
  /**
   * @member {Type} [type] Type of the action. Possible values include:
   * 'ViewAction', 'OpenUri', 'HttpPOST', 'ActionCard'
   */
  '@type'?: O365ConnectorCardActionType;
  /**
   * @member {string} [name] Name of the action that will be used as button
   * title
   */
  name?: string;
  /**
   * @member {string} [id] Action Id
   */
  '@id'?: string;
}

/**
 * Defines values for Type1.
 * Possible values include: 'textInput', 'dateInput', 'multichoiceInput'
 * @readonly
 * @enum {string}
 */
export type O365ConnectorCardInputType = 'textInput' | 'dateInput' | 'multichoiceInput';

/**
 * @interface
 * An interface representing O365ConnectorCardInputBase.
 * O365 connector card input for ActionCard action
 *
 */
export interface O365ConnectorCardInputBase {
  /**
   * @member {Type1} [type] Input type name. Possible values include:
   * 'textInput', 'dateInput', 'multichoiceInput'
   */
  '@type'?: O365ConnectorCardInputType;
  /**
   * @member {string} [id] Input Id. It must be unique per entire O365
   * connector card.
   */
  id?: string;
  /**
   * @member {boolean} [isRequired] Define if this input is a required field.
   * Default value is false.
   */
  isRequired?: boolean;
  /**
   * @member {string} [title] Input title that will be shown as the placeholder
   */
  title?: string;
  /**
   * @member {string} [value] Default value for this input field
   */
  value?: string;
}

export interface TeamsAttachment<ContentType> extends builder.Attachment {
  content: ContentType;
}

export type FileDownloadInfoAttachment = TeamsAttachment<teams.FileDownloadInfo>;

/**
 * @interface
 * An interface representing MessageActionsPayloadBody.
 * Plaintext/HTML representation of the content of the message.
 *
 */
export interface MessageActionsPayloadBody {
  /**
   * @member {ContentType} [contentType] Type of the content. Possible values
   * include: 'html', 'text'
   */
  contentType?: teams.ContentType;
  /**
   * @member {string} [content] The content of the body.
   */
  content?: string;
  /**
   * @member {string} [textContent] The text content of the body after
   * stripping HTML tags.
   */
  textContent?: string;
}
