import { RouterDialog } from "../shared/routerDialog";
import { DialogContext } from "botbuilder-dialogs";
import { BotServices } from "../../botServices";
import { ConversationState, UserState } from "botbuilder";
import { MainResponses } from "./mainResponses";
import { OnboardingState } from "../onboarding/onboardingState";

export class MainDialog extends RouterDialog {
    private readonly _botServices: BotServices;
    private readonly _conversationState: ConversationState;
    private readonly _userState: UserState;
    private readonly _responder: MainResponses;

    constructor(botServices: BotServices, conversationState: ConversationState, userState: UserState) {
        super('MainDialog');
        if (!botServices) throw ('Missing parameter.  botServices is required');
        if (!conversationState) throw ('Missing parameter.  conversationState is required');
        if (!userState) throw ('Missing parameter.  userState is required');

        this._botServices = botServices;
        this._conversationState = conversationState;
        this._userState = userState;

        this._responder = new MainResponses();

        //this.addDialog(new OnboardingDialog(this._botServices, this._userState.createProperty('OnboardingState')));
        //this.addDialog(new EscalateDialog(this._botServices));
    }

    protected async onStart(innerDC: DialogContext): Promise<void> {
        const onboardingAccessor = this._userState.createProperty<OnboardingState>('OnboardingState');
        const onboardingState = await onboardingAccessor.get(innerDC.context, undefined);

        await this._responder.ReplyWith(innerDC.context, MainResponses.Intro);

        if (onboardingState && onboardingState.name) {
            // This is the first time the user is interacting with the bot, so gather onboarding information.
            await innerDC.beginDialog('OnboardingDialog');
        }
    }

    protected route(innerDC: DialogContext): Promise<void> {
        throw new Error("Method not implemented.");
    }

    protected complete(innerDC: DialogContext): Promise<void> {
        throw new Error("Method not implemented.");
    }
}