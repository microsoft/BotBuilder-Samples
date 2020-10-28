// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import axios from 'axios';
const querystring = require( 'querystring' );
import {
  CardFactory,
  TeamsActivityHandler,
  TurnContext
} from 'botbuilder';

export class TeamsMessagingExtensionsSearchBot extends TeamsActivityHandler {
  public async handleTeamsMessagingExtensionQuery( context: TurnContext, query: any ): Promise<any> {
    const searchQuery = query.parameters[ 0 ].value;
    const response = await axios.get( `http://registry.npmjs.com/-/v1/search?${ querystring.stringify( { text: searchQuery, size: 8 } ) }` );

    const attachments = [];
    response.data.objects.forEach( ( obj: any ) => {
      const heroCard = CardFactory.heroCard( obj.package.name );
      const preview = CardFactory.heroCard( obj.package.name );
      preview.content.tap = { type: 'invoke', value: { description: obj.package.description } };
      const attachment = { ...heroCard, preview };
      attachments.push( attachment );
    } );

    return {
      composeExtension: {
        attachmentLayout: 'list',
        attachments,
        type: 'result'
      }
    };
  }

  public async handleTeamsMessagingExtensionSelectItem( context: TurnContext, obj: any ): Promise<any> {
    return {
      composeExtension: {
        attachmentLayout: 'list',
        attachments: [ CardFactory.thumbnailCard( obj.description ) ],
        type: 'result'
      }
    };
  }
}
