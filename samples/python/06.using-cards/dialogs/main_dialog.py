# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License.

from botbuilder.core import CardFactory, MessageFactory
from botbuilder.dialogs import (
    ComponentDialog,
    WaterfallDialog,
    WaterfallStepContext,
)
from botbuilder.dialogs.prompts import TextPrompt, PromptOptions
from botbuilder.schema import (
    ActionTypes,
    Attachment,
    AnimationCard,
    AudioCard,
    HeroCard,
    VideoCard,
    ReceiptCard,
    SigninCard,
    ThumbnailCard,
    MediaUrl,
    CardAction,
    CardImage,
    ThumbnailUrl,
    Fact,
    ReceiptItem,
)

from helpers.activity_helper import create_activity_reply
from .resources.adaptive_card_example import ADAPTIVE_CARD_CONTENT

MAIN_WATERFALL_DIALOG = "mainWaterfallDialog"


class MainDialog(ComponentDialog):
    def __init__(self):
        super().__init__("MainDialog")

        # Define the main dialog and its related components.
        self.add_dialog(TextPrompt("TextPrompt"))
        self.add_dialog(
            WaterfallDialog(
                MAIN_WATERFALL_DIALOG, [self.choice_card_step, self.show_card_step]
            )
        )

        # The initial child Dialog to run.
        self.initial_dialog_id = MAIN_WATERFALL_DIALOG

    async def choice_card_step(self, step_context: WaterfallStepContext):
        """
        1. Prompts the user if the user is not in the middle of a dialog.
        2. Re-prompts the user when an invalid input is received.
        """
        menu_text = (
            "Which card would you like to see?\n"
            "(1) Adaptive Card\n"
            "(2) Animation Card\n"
            "(3) Audio Card\n"
            "(4) Hero Card\n"
            "(5) Receipt Card\n"
            "(6) Signin Card\n"
            "(7) Thumbnail Card\n"
            "(8) Video Card\n"
            "(9) All Cards"
        )

        # Prompt the user with the configured PromptOptions.
        return await step_context.prompt(
            "TextPrompt", PromptOptions(prompt=MessageFactory.text(menu_text))
        )

    async def show_card_step(self, step_context: WaterfallStepContext):
        """
        Send a Rich Card response to the user based on their choice.
        self method is only called when a valid prompt response is parsed from the user's
        response to the ChoicePrompt.
        """
        response = step_context.result.lower().strip()
        choice_dict = {
            "1": [self.create_adaptive_card],
            "adaptive card": [self.create_adaptive_card],
            "2": [self.create_animation_card],
            "animation card": [self.create_animation_card],
            "3": [self.create_audio_card],
            "audio card": [self.create_audio_card],
            "4": [self.create_hero_card],
            "hero card": [self.create_hero_card],
            "5": [self.create_receipt_card],
            "receipt card": [self.create_receipt_card],
            "6": [self.create_signin_card],
            "signin card": [self.create_signin_card],
            "7": [self.create_thumbnail_card],
            "thumbnail card": [self.create_thumbnail_card],
            "8": [self.create_video_card],
            "video card": [self.create_video_card],
            "9": [
                self.create_adaptive_card,
                self.create_animation_card,
                self.create_audio_card,
                self.create_hero_card,
                self.create_receipt_card,
                self.create_signin_card,
                self.create_thumbnail_card,
                self.create_video_card,
            ],
            "all cards": [
                self.create_adaptive_card,
                self.create_animation_card,
                self.create_audio_card,
                self.create_hero_card,
                self.create_receipt_card,
                self.create_signin_card,
                self.create_thumbnail_card,
                self.create_video_card,
            ],
        }

        # Get the functions that will generate the card(s) for our response
        # If the stripped response from the user is not found in our choice_dict, default to None
        choice = choice_dict.get(response, None)
        # If the user's choice was not found, respond saying the bot didn't understand the user's response.
        if not choice:
            not_found = create_activity_reply(
                step_context.context.activity, "Sorry, I didn't understand that. :("
            )
            await step_context.context.send_activity(not_found)
        else:
            for func in choice:
                card = func()
                response = create_activity_reply(
                    step_context.context.activity, "", "", [card]
                )
                await step_context.context.send_activity(response)

        # Give the user instructions about what to do next
        await step_context.context.send_activity("Type anything to see another card.")

        return await step_context.end_dialog()

    # ======================================
    # Helper functions used to create cards.
    # ======================================

    # Methods to generate cards
    def create_adaptive_card(self) -> Attachment:
        return CardFactory.adaptive_card(ADAPTIVE_CARD_CONTENT)

    def create_animation_card(self) -> Attachment:
        card = AnimationCard(
            media=[MediaUrl(url="http://i.giphy.com/Ki55RUbOV5njy.gif")],
            title="Microsoft Bot Framework",
            subtitle="Animation Card",
        )
        return CardFactory.animation_card(card)

    def create_audio_card(self) -> Attachment:
        card = AudioCard(
            media=[MediaUrl(url="http://www.wavlist.com/movies/004/father.wav")],
            title="I am your father",
            subtitle="Star Wars: Episode V - The Empire Strikes Back",
            text="The Empire Strikes Back (also known as Star Wars: Episode V – The Empire Strikes "
            "Back) is a 1980 American epic space opera film directed by Irvin Kershner. Leigh "
            "Brackett and Lawrence Kasdan wrote the screenplay, with George Lucas writing the "
            "film's story and serving as executive producer. The second installment in the "
            "original Star Wars trilogy, it was produced by Gary Kurtz for Lucasfilm Ltd. and "
            "stars Mark Hamill, Harrison Ford, Carrie Fisher, Billy Dee Williams, Anthony "
            "Daniels, David Prowse, Kenny Baker, Peter Mayhew and Frank Oz.",
            image=ThumbnailUrl(
                url="https://upload.wikimedia.org/wikipedia/en/3/3c/SW_-_Empire_Strikes_Back.jpg"
            ),
            buttons=[
                CardAction(
                    type=ActionTypes.open_url,
                    title="Read more",
                    value="https://en.wikipedia.org/wiki/The_Empire_Strikes_Back",
                )
            ],
        )
        return CardFactory.audio_card(card)

    def create_hero_card(self) -> Attachment:
        card = HeroCard(
            title="",
            images=[
                CardImage(
                    url="https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg"
                )
            ],
            buttons=[
                CardAction(
                    type=ActionTypes.open_url,
                    title="Get Started",
                    value="https://docs.microsoft.com/en-us/azure/bot-service/",
                )
            ],
        )
        return CardFactory.hero_card(card)

    def create_video_card(self) -> Attachment:
        card = VideoCard(
            title="Big Buck Bunny",
            subtitle="by the Blender Institute",
            text="Big Buck Bunny (code-named Peach) is a short computer-animated comedy film by the Blender "
            "Institute, part of the Blender Foundation. Like the foundation's previous film Elephants "
            "Dream, the film was made using Blender, a free software application for animation made by "
            "the same foundation. It was released as an open-source film under Creative Commons License "
            "Attribution 3.0.",
            media=[
                MediaUrl(
                    url="http://download.blender.org/peach/bigbuckbunny_movies/"
                    "BigBuckBunny_320x180.mp4"
                )
            ],
            buttons=[
                CardAction(
                    type=ActionTypes.open_url,
                    title="Learn More",
                    value="https://peach.blender.org/",
                )
            ],
        )
        return CardFactory.video_card(card)

    def create_receipt_card(self) -> Attachment:
        card = ReceiptCard(
            title="John Doe",
            facts=[
                Fact(key="Order Number", value="1234"),
                Fact(key="Payment Method", value="VISA 5555-****"),
            ],
            items=[
                ReceiptItem(
                    title="Data Transfer",
                    price="$38.45",
                    quantity="368",
                    image=CardImage(
                        url="https://github.com/amido/azure-vector-icons/raw/master/"
                        "renders/traffic-manager.png"
                    ),
                ),
                ReceiptItem(
                    title="App Service",
                    price="$45.00",
                    quantity="720",
                    image=CardImage(
                        url="https://github.com/amido/azure-vector-icons/raw/master/"
                        "renders/cloud-service.png"
                    ),
                ),
            ],
            tax="$7.50",
            total="90.95",
            buttons=[
                CardAction(
                    type=ActionTypes.open_url,
                    title="More Information",
                    value="https://azure.microsoft.com/en-us/pricing/details/bot-service/",
                )
            ],
        )
        return CardFactory.receipt_card(card)

    def create_signin_card(self) -> Attachment:
        card = SigninCard(
            text="BotFramework Sign-in Card",
            buttons=[
                CardAction(
                    type=ActionTypes.signin,
                    title="Sign-in",
                    value="https://login.microsoftonline.com",
                )
            ],
        )
        return CardFactory.signin_card(card)

    def create_thumbnail_card(self) -> Attachment:
        card = ThumbnailCard(
            title="BotFramework Thumbnail Card",
            subtitle="Your bots — wherever your users are talking",
            text="Build and connect intelligent bots to interact with your users naturally wherever"
            " they are, from text/sms to Skype, Slack, Office 365 mail and other popular services.",
            images=[
                CardImage(
                    url="https://sec.ch9.ms/ch9/7ff5/"
                    "e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/"
                    "buildreactionbotframework_960.jpg"
                )
            ],
            buttons=[
                CardAction(
                    type=ActionTypes.open_url,
                    title="Get Started",
                    value="https://docs.microsoft.com/en-us/azure/bot-service/",
                )
            ],
        )
        return CardFactory.thumbnail_card(card)
