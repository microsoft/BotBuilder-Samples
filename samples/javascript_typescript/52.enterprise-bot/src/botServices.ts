// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

import { TelemetryClient } from 'applicationinsights';
import { BotConfiguration, ServiceTypes, GenericService, AppInsightsService, DispatchService, LuisService, QnaMakerService } from 'botframework-config';
import { TelemetryLuisRecognizer } from './middleware/telemetry/telemetryLuisRecognizer';
import { TelemetryQnAMaker } from './middleware/telemetry/telemetryQnAMaker';
import { LuisApplication, QnAMakerEndpoint } from 'botbuilder-ai';

/**
 * Represents references to external services.
 * For example, LUIS services are kept here as a singleton. This external service is configured
 * using the BotConfiguration class.
 */
export class BotServices {
    private _authConnectionName: string = '';
    private _telemetryClient!: TelemetryClient;
    private _dispatchRecognizer!: TelemetryLuisRecognizer;
    private _luisServices: Map<string, TelemetryLuisRecognizer>;
    private _qnaServices: Map<string, TelemetryQnAMaker>;

    /**
     * Initializes a new instance of the BotServices class.
     * @constructor
     * @param {BotConfiguration} config The BotConfiguration instance for the bot.
     */
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

    /**
     * Gets the set of the Authentication Connection Name for the Bot application.
     * The Authentication Connection Name  should not be modified while the bot is running.
     */
    public get authConnectionName(): string {
        return this._authConnectionName;
    }

    /**
     * Gets the set of AppInsights Telemetry Client used.
     * The AppInsights Telemetry Client should not be modified while the bot is running.
     */
    public get telemetryClient(): TelemetryClient {
        return this._telemetryClient;
    }

    /**
     * Gets the set of Dispatch LUIS Recognizer used.
     * The Dispatch LUIS Recognizer should not be modified while the bot is running.
     */
    public get dispatchRecognizer(): TelemetryLuisRecognizer {
        return this._dispatchRecognizer;
    }

    /**
     * Gets the set of LUIS Services used.
     * Given there can be multiple TelemetryLuisRecognizer services used in a single bot,
     * LuisServices is represented as a dictionary. This is also modeled in the ".bot" file
     * since the elements are named.
     */
    public get luisServices(): Map<string, TelemetryLuisRecognizer> {
        return this._luisServices;
    }

    /**
     * Gets the set of QnAMaker Services used.
     * Given there can be multiple TelemetryQnAMaker services used in a single bot,
     * QnAServices is represented as a dictionary. This is also modeled in the ".bot" file
     * since the elements are named.
     */
    public get qnaServices(): Map<string, TelemetryQnAMaker> {
        return this._qnaServices;
    }
}