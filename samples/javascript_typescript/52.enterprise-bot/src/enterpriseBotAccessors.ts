import { UserState, ConversationState, StatePropertyAccessor } from 'botbuilder';
import { DialogState } from 'botbuilder-dialogs';

export class EnterpriseBotAccessors {
    private _userState: UserState;
    private _conversationState: ConversationState;
    private _dialogStateProperty!: StatePropertyAccessor<DialogState>;

    constructor(userState: UserState, conversationState: ConversationState) {
        if (!userState) throw ('Missing parameter.  userState is required'); 
        if (!conversationState) throw ('Missing parameter.  conversationState is required');
        this._userState = userState;
        this._conversationState = conversationState;
    }

    public static readonly DialogStateName: string = 'EnterpriseBotAccessors.DialogState';

    public get userState(): UserState {
        return this._userState;
    }

    public get conversationState(): ConversationState {
        return this._conversationState;
    }

    public get dialogStateProperty(): StatePropertyAccessor<DialogState> {
        return this._dialogStateProperty;
    }

    public set dialogStateProperty(dialogStateProperty: StatePropertyAccessor<DialogState>) {
        this._dialogStateProperty = dialogStateProperty;
    }
}