// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

import { ConversationState, StatePropertyAccessor, UserState } from "botbuilder";
import { LuisRecognizer } from "botbuilder-ai";
import { DialogContext } from "botbuilder-dialogs";
import { BotServices } from "../../botServices";
import { EscalateDialog } from "../escalate/escalateDialog";
import { OnboardingDialog } from "../onboarding/onboardingDialog";
import { OnboardingState } from "../onboarding/onboardingState";
import { RouterDialog } from "../shared/routerDialog";
import { MainResponses } from "./mainResponses";

export class MainDialog extends RouterDialog {
    private readonly _services: BotServices;
    private readonly _userState: UserState;
    private readonly _conversationState: ConversationState;
    private readonly _responder: MainResponses = new MainResponses();
    private readonly _onboardingAccessor: StatePropertyAccessor<OnboardingState>;

    constructor(services: BotServices, conversationState: ConversationState, userState: UserState) {
        super("MainDialog");
        if (!services) { throw new Error(("Missing parameter.  botServices is required")); }
        if (!conversationState) { throw new Error(("Missing parameter.  conversationState is required")); }
        if (!userState) { throw new Error(("Missing parameter.  userState is required")); }

        this._services = services;
        this._conversationState = conversationState;
        this._userState = userState;

        this._onboardingAccessor = this._userState.createProperty<OnboardingState>("OnboardingState");

        this.addDialog(new OnboardingDialog(this._services, this._onboardingAccessor));
        this.addDialog(new EscalateDialog(this._services));
    }

    protected async onStart(innerDC: DialogContext): Promise<void> {
        const onboardingState = await this._onboardingAccessor.get(innerDC.context, undefined);

        await this._responder.replyWith(innerDC.context, MainResponses.Intro);

        if (!onboardingState || !onboardingState.name) {
            // This is the first time the user is interacting with the bot, so gather onboarding information.
            await innerDC.beginDialog("OnboardingDialog");
        }
    }

    protected async route(dc: DialogContext): Promise<void> {
        // Check dispatch result
        const dispatchResult = await this._services.dispatchRecognizer.recognize(dc.context);
        const topIntent = LuisRecognizer.topIntent(dispatchResult);

        if (topIntent === "l_General") {
            // If dispatch result is general luis model
            const luisService = this._services.luisServices.get(process.env.LUIS_GENERAL || "");
            if (!luisService) { return Promise.reject(new Error("Luis service not found")); }

            const luisResult = await luisService.recognize(dc.context);

            const generalIntent = LuisRecognizer.topIntent(luisResult);

            // switch on general intents
            switch (generalIntent) {
                case "Greeting": {
                    // Send greeting response
                    await this._responder.replyWith(dc.context, MainResponses.Greeting);
                    break;
                }
                case "Help": {
                    // Send help response
                    await this._responder.replyWith(dc.context, MainResponses.Help);
                    break;
                }
                case "Cancel": {
                    // Send cancelled response.
                    await this._responder.replyWith(dc.context, MainResponses.Cancelled);

                    // Cancel any active dialogs on the stack.
                    await dc.cancelAllDialogs();
                    break;
                }
                case "Escalate": {
                    // Start escalate dialog.
                    await dc.beginDialog("EscalateDialog");
                    break;
                }
                default: {
                    // No intent was identified, send confused message.
                    await this._responder.replyWith(dc.context, MainResponses.Confused);
                    break;
                }
            }
        } else if (topIntent === "q_FAQ") {
            const qnaService = this._services.qnaServices.get("FAQ");
            if (!qnaService) { return Promise.reject(new Error("QnA service not found")); }

            const answers = await qnaService.getAnswersAsync(dc.context);

            if (answers && answers.length !== 0) {
                await dc.context.sendActivity(answers[0].answer);
            }
        }
    }

    protected complete(innerDC: DialogContext): Promise<void> {
        // The active dialogs stack ended with a complete status.
        return this._responder.replyWith(innerDC.context, MainResponses.Completed);
    }
}
