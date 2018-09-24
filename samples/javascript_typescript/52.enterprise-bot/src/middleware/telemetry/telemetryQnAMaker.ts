import { QnAMaker, QnAMakerEndpoint, QnAMakerOptions, QnAMakerResult } from 'botbuilder-ai';
import { TurnContext } from 'botbuilder';

export class TelemetryQnAMaker extends QnAMaker {
    private readonly _logOriginalMessage: boolean;
    private readonly _logUsername: boolean;
    private _options: { top: number, scoreThreshold: number } = { top: 1, scoreThreshold: 0.3 };

    /**
     *
     */
    constructor(endpoint: QnAMakerEndpoint, options?: QnAMakerOptions, logUserName: boolean = false, logOriginalMessage: boolean = false) {
        super(endpoint, options);
        this._logOriginalMessage = logOriginalMessage;
        this._logUsername = logUserName;
        Object.assign(this._options, options);   
    }

    public get logOriginalMessage(): boolean { return this._logOriginalMessage; }

    public get logUsername(): boolean { return this._logUsername; }

    public async getAnswersAsync(context: TurnContext): Promise<QnAMakerResult[]> {
        // Call Qna Maker
        const queryResults: QnAMakerResult[] = await super.generateAnswer(context.activity.text, this._options.top, this._options.scoreThreshold);

        // Find the Application Insights Telemetry Client
        // TODO: add after TelemetryLoggerMiddleware

        return queryResults;
    }
}