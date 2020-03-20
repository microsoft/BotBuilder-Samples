# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.
import json
from typing import List

from jsonpickle import encode
from botbuilder.dialogs import (
    ComponentDialog,
    DialogContext,
    WaterfallDialog,
    WaterfallStepContext,
    DialogTurnResult,
)
from botbuilder.dialogs.choices import Choice, FoundChoice
from botbuilder.dialogs.prompts import (
    PromptOptions,
    ChoicePrompt,
    PromptValidatorContext,
)
from botbuilder.dialogs.skills import (
    SkillDialogOptions,
    SkillDialog,
    BeginSkillDialogOptions,
)
from botbuilder.core import ConversationState, MessageFactory, TurnContext
from botbuilder.core.skills import BotFrameworkSkill, ConversationIdFactoryBase
from botbuilder.schema import Activity, ActivityTypes, InputHints, DeliveryModes
from botbuilder.integration.aiohttp.skills import SkillHttpClient

from config import SkillConfiguration, DefaultConfig


class MainDialog(ComponentDialog):

    ACTIVE_SKILL_PROPERTY_NAME = f"MainDialog.ActiveSkillProperty"

    def __init__(
        self,
        conversation_state: ConversationState,
        conversation_id_factory: ConversationIdFactoryBase,
        skill_client: SkillHttpClient,
        skills_config: SkillConfiguration,
        configuration: DefaultConfig,
    ):
        super(MainDialog, self).__init__(MainDialog.__name__)

        # Constants used for selecting actions on the skill.
        self._skill_action_book_flight = "BookFlight"
        self._skill_action_book_flight_with_input_parameters = (
            "BookFlight with input parameters"
        )
        self._skill_action_get_weather = "GetWeather"
        self._skill_action_message = "Message"

        self._selected_skill_key = (
            f"{MainDialog.__module__}.{MainDialog.__name__}.SelectedSkillKey"
        )

        bot_id = configuration.APP_ID
        if not bot_id:
            raise TypeError("App Id is not in configuration")

        self._skills_config = skills_config
        if not self._skills_config:
            raise TypeError("Skills configuration cannot be None")

        if not skill_client:
            raise TypeError("skill_client cannot be None")

        if not conversation_state:
            raise TypeError("conversation_state cannot be None")

        # Use helper method to add SkillDialog instances for the configured skills.
        self._add_skill_dialogs(
            conversation_state,
            conversation_id_factory,
            skill_client,
            skills_config,
            bot_id,
        )

        # ChoicePrompt to render available skills
        self.add_dialog(ChoicePrompt("SkillPrompt"))

        # ChoicePrompt to render skill actions
        self.add_dialog(
            ChoicePrompt("SkillActionPrompt", self._skill_action_prompt_validator)
        )

        # Main waterfall dialog for this bot
        self.add_dialog(
            WaterfallDialog(
                WaterfallDialog.__name__,
                [
                    self._select_skill_step,
                    self._select_skill_action_step,
                    self._call_skill_action_step,
                    self._final_step,
                ],
            )
        )

        self._active_skill_property = conversation_state.create_property(
            MainDialog.ACTIVE_SKILL_PROPERTY_NAME
        )

        self.initial_dialog_id = WaterfallDialog.__name__

    async def on_continue_dialog(self, inner_dc: DialogContext) -> DialogTurnResult:
        # This is an example on how to cancel a SkillDialog that is currently in progress from the parent bot
        active_skill = await self._active_skill_property.get(inner_dc.context)
        activity = inner_dc.context.activity

        if (
            active_skill
            and activity.type == ActivityTypes.message
            and "abort" in activity.text
        ):
            # Cancel all dialog when the user says abort.
            await inner_dc.cancel_all_dialogs()
            return await inner_dc.replace_dialog(self.initial_dialog_id)

        return await super().on_continue_dialog(inner_dc)

    async def _select_skill_step(
        self, step_context: WaterfallStepContext
    ) -> DialogTurnResult:
        # Create the PromptOptions from the skill configuration which contain the list of configured skills.
        message_text = (
            str(step_context.options)
            if step_context.options
            else "What skill would you like to call?"
        )
        reprompt_text = "That was not a valid choice, please select a valid skill."
        options = PromptOptions(
            prompt=MessageFactory.text(message_text),
            retry_prompt=MessageFactory.text(reprompt_text),
            choices=[
                Choice(value=skill.id)
                for _, skill in self._skills_config.SKILLS.items()
            ],
        )

        # Prompt the user to select a skill.
        return await step_context.prompt("SkillPrompt", options)

    async def _select_skill_action_step(
        self, step_context: WaterfallStepContext
    ) -> DialogTurnResult:
        # Get the skill info based on the selected skill.
        selected_skill_id = step_context.result.value
        selected_skill = self._skills_config.SKILLS.get(selected_skill_id)

        # Remember the skill selected by the user.
        step_context.values[self._selected_skill_key] = selected_skill

        # Create the PromptOptions with the actions supported by the selected skill.
        message_text = f"Select an action # to send to **{selected_skill.id}** or just type in a message and it will be forwarded to the skill"
        options = PromptOptions(
            prompt=MessageFactory.text(
                message_text, message_text, InputHints.expecting_input
            ),
            choices=self._get_skill_actions(selected_skill),
        )

        # Prompt the user to select a skill action.
        return await step_context.prompt("SkillActionPrompt", options)

    async def _call_skill_action_step(
        self, step_context: WaterfallStepContext
    ) -> DialogTurnResult:
        # Starts SkillDialog based on the user's selections
        selected_skill: BotFrameworkSkill = step_context.values[
            self._selected_skill_key
        ]

        if selected_skill.id == "DialogSkillBot":
            skill_activity = self._create_dialog_skill_bot_activity(
                step_context.result.value, step_context.context
            )
        else:
            raise Exception(f"Unknown target skill id: {selected_skill.id}.")

        # Create the BeginSkillDialogOptions and assign the activity to send.
        skill_dialog_args = BeginSkillDialogOptions(skill_activity)

        # Save active skill in state.
        await self._active_skill_property.set(step_context.context, selected_skill)

        # Start the skillDialog instance with the arguments.
        return await step_context.begin_dialog(selected_skill.id, skill_dialog_args)

    async def _final_step(self, step_context: WaterfallStepContext) -> DialogTurnResult:
        active_skill = await self._active_skill_property.get(step_context.context)

        if step_context.result:
            message = f"Skill {active_skill.id} invocation complete."
            message += f" Result: {encode(step_context.result)}"
            await step_context.context.send_activity(
                MessageFactory.text(message, input_hint=InputHints.ignoring_input)
            )

        # Clear the skill selected by the user.
        step_context.values[self._selected_skill_key] = None

        # Clear active skill in state
        await self._active_skill_property.delete(step_context.context)

        # Restart the main dialog with a different message the second time around
        return await step_context.replace_dialog(
            self.initial_dialog_id,
            f'Done with "{active_skill.id}". \n\n What skill would you like to call?',
        )

    # Helper method that creates and adds SkillDialog instances for the configured skills.
    def _add_skill_dialogs(
        self,
        conversation_state: ConversationState,
        conversation_id_factory: ConversationIdFactoryBase,
        skill_client: SkillHttpClient,
        skills_config: SkillConfiguration,
        bot_id: str,
    ):
        for _, skill_info in skills_config.SKILLS.items():
            # Create the dialog options.
            skill_dialog_options = SkillDialogOptions(
                bot_id=bot_id,
                conversation_id_factory=conversation_id_factory,
                skill_client=skill_client,
                skill_host_endpoint=skills_config.SKILL_HOST_ENDPOINT,
                conversation_state=conversation_state,
                skill=skill_info,
            )

            # Add a SkillDialog for the selected skill.
            self.add_dialog(SkillDialog(skill_dialog_options, skill_info.id))

    # This validator defaults to Message if the user doesn't select an existing option.
    async def _skill_action_prompt_validator(
        self, prompt_context: PromptValidatorContext
    ) -> bool:
        if not prompt_context.recognized.succeeded:
            # Assume the user wants to send a message if an item in the list is not selected.
            prompt_context.recognized.value = FoundChoice(
                self._skill_action_message, None, None
            )

        return True

    # Helper method to create Choice elements for the actions supported by the skill.
    def _get_skill_actions(self, skill: BotFrameworkSkill) -> List[Choice]:
        # Note: the bot would probably render this by reading the skill manifest.
        # We are just using hardcoded skill actions here for simplicity.

        choices = []
        if skill.id == "DialogSkillBot":
            choices.append(Choice(self._skill_action_book_flight))
            choices.append(Choice(self._skill_action_book_flight_with_input_parameters))
            choices.append(Choice(self._skill_action_get_weather))

        return choices

    # Helper method to create the activity to be sent to the DialogSkillBot using selected type and values.
    def _create_dialog_skill_bot_activity(
        self, selected_option: str, turn_context: TurnContext
    ) -> Activity:
        selected_option = selected_option.lower()
        # Note: in a real bot, the dialogArgs will be created dynamically based on the conversation
        # and what each action requires; here we hardcode the values to make things simpler.

        # Just forward the message activity to the skill with whatever the user said.
        if selected_option == self._skill_action_message.lower():
            # Note message activities also support input parameters but we are not using them in this example.
            return turn_context.activity

        activity = None

        # Send an event activity to the skill with "BookFlight" in the name.
        if selected_option == self._skill_action_book_flight.lower():
            activity = Activity(type=ActivityTypes.event)
            activity.name = self._skill_action_book_flight

        # Send an event activity to the skill with "BookFlight" in the name and some testing values.
        if (
            selected_option
            == self._skill_action_book_flight_with_input_parameters.lower()
        ):
            activity = Activity(type=ActivityTypes.event)
            activity.name = self._skill_action_book_flight
            activity.value = {"origin": "New York", "destination": "Seattle"}

        # Send an event activity to the skill with "GetWeather" in the name and some testing values.
        if selected_option == self._skill_action_get_weather.lower():
            activity = Activity(type=ActivityTypes.event)
            activity.name = self._skill_action_get_weather
            activity.value = {"latitude": 47.614891, "longitude": -122.195801}
            return activity

        if not activity:
            raise Exception(f"Unable to create dialogArgs for {selected_option}.")

        # We are manually creating the activity to send to the skill; ensure we add the ChannelData and Properties
        # from the original activity so the skill gets them.
        # Note: this is not necessary if we are just forwarding the current activity from context.
        activity.channel_data = turn_context.activity.channel_data
        activity.additional_properties = turn_context.activity.additional_properties

        return activity
