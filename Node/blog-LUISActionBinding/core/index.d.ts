import { Session, IntentDialog, UniversalBot } from "botbuilder";

// CONSTS
export namespace Status {
    export const NoActionRecognized: string;
    export const Fulfilled: string;
    export const MissingParameters: string;
    export const ContextSwitch: string;
}

// API
export function evaluate(
    modelUrl: string,
    actions: Array<IAction>,
    currentActionModel?: IActionModel,
    userInput?: string,
    onContextCreationHandler?: onContextCreationHandler): PromiseLike<IActionModel>;

declare type onContextCreationHandler = (action: IAction, actionModel: IActionModel, next: () => void) => void

export function bindToBotDialog(
    bot: UniversalBot,
    intentDialog: IntentDialog,
    modelUrl: string,
    actions: Array<IAction>,
    options: IBindToDialogOptions
)

declare type onDialogContextCreationHandler = (action: IAction, actionModel: IActionModel, next: () => void, session: Session) => void
declare type replyHandler = (session: Session) => void
declare type fulfillHandler = (session: Session, actionModel: IActionModel) => void

export interface IBindToDialogOptions {
    defaultReply: replyHandler,
    fulfillReply: fulfillHandler,
    onContextCreation: onDialogContextCreationHandler    
}

// TYPES
export interface IAction {
    intentName: string;
    friendlyName: string;
    confirmOnContextSwitch?: boolean;
    canExecuteWithoutContext?: boolean;
    parentAction?: IAction,
    schema: { [key: string]: ISchemaParameter };
    fulfill: (parameters: any, callback: (result: any) => void) => void;
}

export interface ISchemaParameter {
    type: string;
    builtInType?: string;
    message: string;
}

export interface IActionModel {
    status: string;
    intentName: string;
    result?: any;
    userInput?: string;
    currentParameter?: string;
    parameters: { [key: string]: any };
    parameterErrors: Array<IParameterError>;
    contextSwitchPrompt?: string,
    confirmSwitch?: boolean;
    subcontextResult?: any;
}

export interface IParameterError {
    parameterName: string;
    message: string;
}