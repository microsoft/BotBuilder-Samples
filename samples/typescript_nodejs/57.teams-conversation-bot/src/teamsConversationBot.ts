// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import {
    ActionTypes,
    BotFrameworkAdapter,
    CardFactory,
    ChannelAccount,
    MessageFactory,
    TeamInfo,
    TeamsActivityHandler,
    TeamsInfo,
    TurnContext
} from 'botbuilder';
const TextEncoder = require( 'util' ).TextEncoder;

export class TeamsConversationBot extends TeamsActivityHandler {
    constructor() {
        super();
        this.onMessage( async ( context: TurnContext, next ): Promise<void> => {
            TurnContext.removeRecipientMention( context.activity );
            const text = context.activity.text.trim().toLocaleLowerCase();
            if ( text.includes( 'mention' ) ) {
                await this.mentionActivityAsync( context );
            } else if ( text.includes( 'update' ) ) {
                await this.cardActivityAsync( context, true );
            } else if ( text.includes( 'delete' ) ) {
                await this.deleteCardActivityAsync( context );
            } else if ( text.includes( 'message' ) ) {
                await this.messageAllMembersAsync( context );
            } else if ( text.includes( 'who' ) ) {
                await this.getSingleMember( context );
            } else {
                await this.cardActivityAsync( context, false );
            }
            await next();
        } );

        this.onTeamsMembersAddedEvent( async ( membersAdded: ChannelAccount[], teamInfo: TeamInfo, context: TurnContext, next: () => Promise<void> ): Promise<void> => {
            let newMembers: string = '';
            membersAdded.forEach( ( account ) => {
                newMembers += account.id + ' ';
            } );
            const name = !teamInfo ? 'not in team' : teamInfo.name;
            const card = CardFactory.heroCard( 'Account Added', `${ newMembers } joined ${ name }.` );
            const message = MessageFactory.attachment( card );
            await context.sendActivity( message );
            await next();
        } );
    }

    public async cardActivityAsync( context: TurnContext, isUpdate ): Promise<void> {
        const cardActions = [
            {
                text: 'MessageAllMembers',
                title: 'Message all members',
                type: ActionTypes.MessageBack,
                value: null
            },
            {
                text: 'whoAmI',
                title: 'Who am I?',
                type: ActionTypes.MessageBack,
                value: null
            },
            {
                text: 'Delete',
                title: 'Delete card',
                type: ActionTypes.MessageBack,
                value: null
            }
        ];

        if ( isUpdate ) {
            await this.sendUpdateCard( context, cardActions );
        } else {
            await this.sendWelcomeCard( context, cardActions );
        }
    }

    public async sendUpdateCard( context: TurnContext, cardActions ): Promise<void> {
        const data = context.activity.value;
        data.count += 1;
        cardActions.push( {
            text: 'UpdateCardAction',
            title: 'Update Card',
            type: ActionTypes.MessageBack,
            value: data
        } );
        const card = CardFactory.heroCard(
            'Updated card',
            `Update count: ${ data.count }`,
            null,
            cardActions
        );
        // card.id = context.activity.replyToId;
        const message = MessageFactory.attachment( card );
        message.id = context.activity.replyToId;
        await context.updateActivity( message );
    }

    public async sendWelcomeCard( context: TurnContext, cardActions ): Promise<void> {
        const initialValue = {
            count: 0
        };
        cardActions.push( {
            text: 'UpdateCardAction',
            title: 'Update Card',
            type: ActionTypes.MessageBack,
            value: initialValue
        } );
        const card = CardFactory.heroCard(
            'Welcome card',
            '',
            null,
            cardActions
        );
        await context.sendActivity( MessageFactory.attachment( card ) );
    }

    public async getSingleMember( context: TurnContext ): Promise<void> {
        let member;
        try {
            member = await TeamsInfo.getMember( context, context.activity.from.id );
        } catch ( e ) {
            if ( e.code === 'MemberNotFoundInConversation' ) {
                context.sendActivity( MessageFactory.text( 'Member not found.' ) );
                return;
            } else {
                console.log( e );
                throw e;
            }
        }
        const message = MessageFactory.text( `You are: ${ member.name }` );
        await context.sendActivity( message );
    }

    public async mentionActivityAsync( context: TurnContext ): Promise<void> {
        const mention = {
            mentioned: context.activity.from,
            text: `<at>${ new TextEncoder().encode( context.activity.from.name ) }</at>`,
            type: 'mention'
        };

        const replyActivity = MessageFactory.text( `Hi ${ mention.text }` );
        replyActivity.entities = [ mention ];
        await context.sendActivity( replyActivity );
    }

    public async deleteCardActivityAsync( context: TurnContext ): Promise<void> {
        await context.deleteActivity( context.activity.replyToId );
    }

    // If you encounter permission-related errors when sending this message, see
    // https://aka.ms/BotTrustServiceUrl
    public async messageAllMembersAsync( context: TurnContext ): Promise<void> {
        const members = await this.getPagedMembers( context );

        members.forEach( async ( teamMember ) => {
            console.log( 'a ', teamMember );
            const message = MessageFactory.text( `Hello ${ teamMember.givenName } ${ teamMember.surname }. I'm a Teams conversation bot.` );

            const ref = TurnContext.getConversationReference( context.activity );
            ref.user = teamMember;
            let botAdapter: BotFrameworkAdapter;
            botAdapter = context.adapter as BotFrameworkAdapter;
            await botAdapter.createConversation( ref,
                async ( t1 ) => {
                    const ref2 = TurnContext.getConversationReference( t1.activity );
                    await t1.adapter.continueConversation( ref2, async ( t2 ) => {
                        await t2.sendActivity( message );
                    } );
                } );
        } );

        await context.sendActivity( MessageFactory.text( 'All messages have been sent.' ) );
    }

    public async getPagedMembers( context: TurnContext ): Promise<any> {
        let continuationToken;
        const members = [];
        do {
            const pagedMembers = await TeamsInfo.getPagedMembers( context, 100, continuationToken );
            continuationToken = pagedMembers.continuationToken;
            members.push( ...pagedMembers.members );
        } while ( continuationToken !== undefined );
        return members;
    }
}
