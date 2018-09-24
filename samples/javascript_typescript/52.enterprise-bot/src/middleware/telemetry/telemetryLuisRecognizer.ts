import { LuisRecognizer, LuisApplication, LuisPredictionOptions } from 'botbuilder-ai';
import { RecognizerResult, TurnContext } from 'botbuilder';

export class TelemetryLuisRecognizer extends LuisRecognizer {
    private readonly _logOriginalMessage: boolean;
    private readonly _logUsername: boolean;

    /**
     *
     */
    constructor(application: LuisApplication, predictionOptions?: LuisPredictionOptions, includeApiResults: boolean = false, logOriginalMessage: boolean = false, logUserName: boolean = false) {
        super(application, predictionOptions, includeApiResults);
        this._logOriginalMessage = logOriginalMessage;
        this._logUsername = logUserName;    
    }

    public get logOriginalMessage(): boolean { return this._logOriginalMessage; }

    public get logUsername(): boolean { return this._logUsername; }

    public async recognize(context: TurnContext, logOriginalMessage: boolean = false): Promise<RecognizerResult> {
        if (context === null) {
            throw new Error('context is null');
        }

        // Call Luis Recognizer
        const recognizerResult: RecognizerResult = await super.recognize(context);

        const conversationId: string = context.activity.conversation.id;

        // Find the Telemetry Client
        // TODO: add after TelemetryLoggerMiddleware

        return recognizerResult;
    }
}