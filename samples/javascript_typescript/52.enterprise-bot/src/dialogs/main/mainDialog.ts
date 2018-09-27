import { RouterDialog } from "../shared/routerDialog";
import { DialogContext } from "botbuilder-dialogs";
import { BotServices } from "../../botServices";
import { ConversationState, UserState } from "botbuilder";
import { MainResponses } from "./mainResponses";
import { OnboardingState } from "../onboarding/onboardingState";
import { LuisRecognizer } from "botbuilder-ai";

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

    protected async route(innerDC: DialogContext): Promise<void> {
        // Check dispatch result
        const dispatchResult = await this._botServices.dispatchRecognizer.recognize(innerDC.context);
        const topIntent = LuisRecognizer.topIntent(dispatchResult);

        if (topIntent === 'l_General') {
            // If dispatch result is general luis model
            const luisService = this._botServices.luisServices.get('<YOUR MS BOT NAME>_General');
            if (!luisService) return Promise.reject(new Error('Luis service not found'));

            const luisResult = await luisService.recognize(innerDC.context);

            const generalIntent = LuisRecognizer.topIntent(luisResult);

            // switch on general intents
            switch (generalIntent) {
                case 'Greeting': {
                    // Send greeting response
                    await this._responder.ReplyWith(innerDC.context, MainResponses.Greeting);
                    break;
                }
                case 'Help': {
                    // Send help response
                    await this._responder.ReplyWith(innerDC.context, MainResponses.Help);
                    break;
                }
                case 'Cancel': {
                    // Send cancelled response.
                    await this._responder.ReplyWith(innerDC.context, MainResponses.Cancelled);

                    // Cancel any active dialogs on the stack.
                    await innerDC.cancelAllDialogs();
                    break;
                }
                case 'Escalate': {
                    // Start escalate dialog.
                    await innerDC.beginDialog('EscalateDialog');
                    break;
                }
                default: {
                    // No intent was identified, send confused message.
                    await this._responder.ReplyWith(innerDC.context, MainResponses.Confused);
                    break;
                }
            }
        } else if (topIntent === 'q_FAQ') {
            const qnaService = this._botServices.qnaServices.get('EnterpriseBot-FAQ');
            if (!qnaService) return Promise.reject(new Error('QnA service not found'));

            const answers = await qnaService.getAnswersAsync(innerDC.context);

            if (answers && answers.length !== 0) {
                await innerDC.context.sendActivity(answers[0].answer);
            }
        }
    }

    protected async complete(innerDC: DialogContext): Promise<void> {
        // The active dialogs stack ended with a complete status.
        await this._responder.ReplyWith(innerDC.context, MainResponses.Completed);
    }
}