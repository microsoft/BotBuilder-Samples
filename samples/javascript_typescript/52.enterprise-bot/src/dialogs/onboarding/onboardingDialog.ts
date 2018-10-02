// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

import { StatePropertyAccessor, TurnContext } from "botbuilder";
import { DialogTurnResult, TextPrompt, WaterfallDialog, WaterfallStepContext } from "botbuilder-dialogs";
import { BotServices } from "../../botServices";
import { EnterpriseDialog } from "../shared/enterpriseDialog";
import { OnboardingResponses } from "./onboardingResponses";
import { OnboardingState } from "./onboardingState";

export class OnboardingDialog extends EnterpriseDialog {
    private readonly NamePrompt: string = "namePrompt";
    private readonly EmailPrompt: string = "emailPrompt";
    private readonly LocationPrompt: string = "locationPrompt";
    private readonly _responder: OnboardingResponses;
    private readonly _accessor: StatePropertyAccessor<OnboardingState>;
    private _state!: OnboardingState;

    constructor(botServices: BotServices, accessor: StatePropertyAccessor<OnboardingState>) {
        super(botServices, "OnboardingDialog");

        this._accessor = accessor;
        this.initialDialogId = "OnboardingDialog";
        this._responder = new OnboardingResponses();

        this.addDialog(new WaterfallDialog<OnboardingState>(this.initialDialogId, [
            this.askForName.bind(this),
            this.askForEmail.bind(this),
            this.askForLocation.bind(this),
            this.finishOnboardingDialog.bind(this),
        ]));
        this.addDialog(new TextPrompt(this.NamePrompt));
        this.addDialog(new TextPrompt(this.EmailPrompt));
        this.addDialog(new TextPrompt(this.LocationPrompt));

    }

    public async askForName(sc: WaterfallStepContext<OnboardingState>): Promise<DialogTurnResult> {
        this._state = await this.getStateFromAccessor(sc.context);

        if (this._state.name) {
            return await sc.next(this._state.name);
        } else {
            return await sc.prompt(this.NamePrompt, {
                prompt: await this._responder.renderTemplate(sc.context, OnboardingResponses.NamePrompt, "en"),
            });
        }
    }

    public async askForEmail(sc: WaterfallStepContext<OnboardingState>): Promise<DialogTurnResult> {
        this._state = await this.getStateFromAccessor(sc.context);
        this._state.name = sc.result;

        await this._responder.replyWith(sc.context, OnboardingResponses.HaveName, { name: this._state.name });

        if (this._state.email) {
            return await sc.next(this._state.email);
        } else {
            return await sc.prompt(this.EmailPrompt, {
                prompt: await this._responder.renderTemplate(sc.context, OnboardingResponses.EmailPrompt, "en"),
            });
        }
    }

    public async askForLocation(sc: WaterfallStepContext<OnboardingState>): Promise<DialogTurnResult> {
        this._state = await this.getStateFromAccessor(sc.context);
        this._state.email = sc.result;

        await this._responder.replyWith(sc.context, OnboardingResponses.HaveEmail, { email: this._state.email });

        if (this._state.location) {
            return await sc.next(this._state.location);
        } else {
            return await sc.prompt(this.LocationPrompt, {
                prompt: await this._responder.renderTemplate(sc.context, OnboardingResponses.LocationPrompt, "en"),
            });
        }
    }

    public async finishOnboardingDialog(sc: WaterfallStepContext<OnboardingState>): Promise<DialogTurnResult> {
        this._state = await this.getStateFromAccessor(sc.context);
        this._state.location = sc.result;

        await this._responder.replyWith(sc.context, OnboardingResponses.HaveLocation, { name: this._state.name, location: this._state.location });

        return await sc.endDialog();
    }

    private async getStateFromAccessor(context: TurnContext): Promise<OnboardingState>  {
        const state: OnboardingState | undefined = await this._accessor.get(context);
        if (!state) {
            const newState: OnboardingState = {
                email: "",
                location: "",
                name: "",
            };
            await this._accessor.set(context, newState);
            return newState;
        }
        return state;
    }
}
