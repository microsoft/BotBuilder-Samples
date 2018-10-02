// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

import { TelemetryClient } from "applicationinsights";
import { LuisApplication, QnAMakerEndpoint } from "botbuilder-ai";
import { AppInsightsService, BotConfiguration, DispatchService, GenericService, LuisService, QnaMakerService, ServiceTypes } from "botframework-config";
import { TelemetryLuisRecognizer } from "./middleware/telemetry/telemetryLuisRecognizer";
import { TelemetryQnAMaker } from "./middleware/telemetry/telemetryQnAMaker";

/**
 * Represents references to external services.
 * For example, LUIS services are kept here as a singleton. This external service is configured
 * using the BotConfiguration class.
 */
export class BotServices {
    private _authConnectionName: string = "";
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

        config.services.forEach((service) => {
            switch (service.type) {
                case ServiceTypes.AppInsights: {
                    const appInsights: AppInsightsService = service as AppInsightsService;
                    this._telemetryClient = new TelemetryClient(appInsights.instrumentationKey);
                    break;
                }
                case ServiceTypes.Dispatch: {
                    const dispatch: DispatchService = service as DispatchService;
                    const dispatchApp: LuisApplication = {
                        applicationId: dispatch.appId,
                        endpoint: this.getLuisPath(dispatch.region),
                        endpointKey: dispatch.subscriptionKey,
                    };
                    this._dispatchRecognizer = new TelemetryLuisRecognizer(dispatchApp);
                    break;
                }
                case ServiceTypes.Luis: {
                    const luis: LuisService = service as LuisService;
                    const luisApp: LuisApplication = {
                        applicationId: luis.appId,
                        endpoint: this.getLuisPath(luis.region),
                        endpointKey: luis.subscriptionKey,
                    };
                    this._luisServices.set(luis.name, new TelemetryLuisRecognizer(luisApp));
                    break;
                }
                case ServiceTypes.QnA: {
                    const qna: QnaMakerService = service as QnaMakerService;
                    const qnaEndpoint: QnAMakerEndpoint = {
                        endpointKey: qna.endpointKey,
                        host: qna.hostname,
                        knowledgeBaseId: qna.kbId,
                    };
                    this._qnaServices.set(qna.name, new TelemetryQnAMaker(qnaEndpoint));
                    break;
                }
                case ServiceTypes.Generic: {
                    if (service.name === "Authentication") {
                        const authentication: GenericService = service as GenericService;
                        if (authentication.configuration["Azure Active Directory v2"]) {
                            this._authConnectionName = authentication.configuration["Azure Active Directory v2"];
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
