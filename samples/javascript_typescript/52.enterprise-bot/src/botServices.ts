import { TelemetryClient } from 'applicationinsights';
import { BotConfiguration, ServiceTypes, GenericService, AppInsightsService, DispatchService, LuisService, QnaMakerService } from 'botframework-config';
import { TelemetryLuisRecognizer } from './middleware/telemetry/telemetryLuisRecognizer';
import { TelemetryQnAMaker } from './middleware/telemetry/telemetryQnAMaker';
import { LuisApplication, QnAMakerEndpoint } from 'botbuilder-ai';

export class BotServices {
    private _authConnectionName: string = '';
    private _telemetryClient!: TelemetryClient;
    private _dispatchRecognizer!: TelemetryLuisRecognizer;
    private _luisServices: Map<string, TelemetryLuisRecognizer>;
    private _qnaServices: Map<string, TelemetryQnAMaker>;

    constructor(config: BotConfiguration) {
        this._luisServices = new Map<string, TelemetryLuisRecognizer>();
        this._qnaServices = new Map<string, TelemetryQnAMaker>();

        config.services.forEach(service => {
            switch (service.type) {
                case ServiceTypes.AppInsights: {
                    let appInsights: AppInsightsService = <AppInsightsService>service;
                    this._telemetryClient = new TelemetryClient(appInsights.instrumentationKey);
                    break;
                }
                case ServiceTypes.Dispatch: {
                    let dispatch: DispatchService = <DispatchService>service;
                    let dispatchApp: LuisApplication = {
                        applicationId: dispatch.appId,
                        endpointKey: dispatch.subscriptionKey,
                        endpoint: this.getLuisPath(dispatch.region)
                    };
                    this._dispatchRecognizer = new TelemetryLuisRecognizer(dispatchApp);
                    break;
                }
                case ServiceTypes.Luis: {
                    let luis: LuisService = <LuisService>service;
                    let luisApp: LuisApplication = {
                        applicationId: luis.appId,
                        endpointKey: luis.subscriptionKey,
                        endpoint: this.getLuisPath(luis.region)
                    };
                    this._luisServices.set(luis.name, new TelemetryLuisRecognizer(luisApp));
                    break;
                }
                case ServiceTypes.QnA: {
                    let qna: QnaMakerService = <QnaMakerService>service;
                    let qnaEndpoint: QnAMakerEndpoint = {
                        knowledgeBaseId: qna.kbId,
                        endpointKey: qna.endpointKey,
                        host: qna.hostname
                    };
                    this._qnaServices.set(qna.name, new TelemetryQnAMaker(qnaEndpoint));
                    break;
                }
                case ServiceTypes.Generic: {
                    if (service.name === 'Authentication') {
                        let authentication: GenericService = <GenericService> service;
                        if (authentication.configuration['Azure Active Directory v2']) {
                            this._authConnectionName = authentication.configuration['Azure Active Directory v2'];
                        }
                    }
                    break;
                }
            }
        });
    }

    private getLuisPath(region: string): string {
        return `https://${region}.api.cognitive.microsoft.com`;
    }

    public get authConnectionName(): string {
        return this._authConnectionName;
    }

    public get telemetryClient(): TelemetryClient {
        return this._telemetryClient;
    }

    public get dispatchRecognizer(): TelemetryLuisRecognizer {
        return this._dispatchRecognizer;
    }

    public get luisServices(): Map<string, TelemetryLuisRecognizer> {
        return this._luisServices;
    }

    public get qnaServices(): Map<string, TelemetryQnAMaker> {
        return this._qnaServices;
    }
}