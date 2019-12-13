from botbuilder.core import CardFactory, TurnContext, MessageFactory
from botbuilder.core.teams import TeamsActivityHandler, TeamsInfo
from botbuilder.schema import CardAction, HeroCard, Mention
from botbuilder.schema._connector_client_enums import ActionTypes


class TeamsConversationBot(TeamsActivityHandler):
    def __init__(self, app_id: str, app_password: str):
        self._app_id = app_id
        self._app_password = app_password

    async def on_message_activity(self, turn_context: TurnContext):
        TurnContext.remove_recipient_mention(turn_context.activity)
        turn_context.activity.text = turn_context.activity.text.strip()

        if turn_context.activity.text == "MentionMe":
            await self._mention_activity(turn_context)
            return

        if turn_context.activity.text == "UpdateCardAction":
            await self._update_card_activity(turn_context)
            return

        if turn_context.activity.text == "MessageAllMembers":
            await self._message_all_members(turn_context)
            return

        if turn_context.activity.text == "Delete":
            await self._delete_card_activity(turn_context)
            return

        card = HeroCard(
            title="Welcome Card",
            text="Click the buttons to update this card",
            buttons=[
                CardAction(
                        type=ActionTypes.message_back,
                        title="Update Card",
                        text="UpdateCardAction",
                        value={"count": 0}
                ),
                CardAction(
                        type=ActionTypes.message_back,
                        title="Message all memebers",
                        text="MessageAllMembers"
                )
            ]
        )
        await turn_context.send_activity(MessageFactory.attachment(CardFactory.hero_card(card)))
        return

    async def _mention_activity(self, turn_context: TurnContext):
        mention = Mention(
            mentioned=turn_context.activity.from_property,
            text=f"<at>{turn_context.activity.from_property.name}</at>",
            type="mention"
        )

        reply_activity = MessageFactory.text(f"Hello {mention.text}")
        reply_activity.entities = [Mention().deserialize(mention.serialize())]
        await turn_context.send_activity(reply_activity)

    async def _update_card_activity(self, turn_context: TurnContext):
        data = turn_context.activity.value
        data["count"] += 1

        card = CardFactory.hero_card(HeroCard(
            title="Welcome Card",
            text=f"Updated count - {data['count']}",
            buttons=[
                CardAction(
                    type=ActionTypes.message_back,
                    title='Update Card',
                    value=data,
                    text='UpdateCardAction'
                ),
                CardAction(
                    type=ActionTypes.message_back,
                    title='Message all members',
                    text='MessageAllMembers'

                ),
                CardAction(
                    type= ActionTypes.message_back,
                    title='Delete card',
                    text='Delete'
                )
            ]
            )
        )
        
        updated_activity = MessageFactory.attachment(card)
        updated_activity.id = turn_context.activity.reply_to_id
        await turn_context.update_activity(updated_activity)
    
    async def _message_all_members(self, turn_context: TurnContext):
        team_members = await TeamsInfo.get_members(turn_context)

        for member in team_members:
            proactive_message = MessageFactory.text(f"Hello {member.name}. I'm a Teams conversation bot.")

            async def get_ref(tc1):
                ref2 = TurnContext.get_conversation_reference(tc1.activity)
                return await tc1.adapter.continue_conversation(self._app_id, ref2, send_message)

            async def send_message(tc2: TurnContext):
                return tc2

            ref = TurnContext.get_conversation_reference(turn_context.activity)
            result = await turn_context.adapter.create_conversation(ref, get_ref)
            await result.send_activity(proactive_message)
        
        await turn_context.send_activity(MessageFactory.text("All messages have been sent"))
            
           
    
    async def _delete_card_activity(self, turn_context: TurnContext):
        await turn_context.delete_activity(turn_context.activity.reply_to_id)
