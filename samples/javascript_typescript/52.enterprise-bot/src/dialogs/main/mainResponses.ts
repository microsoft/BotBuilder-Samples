// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

import { ActionTypes, Activity, CardFactory, TurnContext } from "botbuilder";
import { ActivityEx } from "../../utils/activityEx";
import { ResourceParser } from "../shared/resourceParser";
import { DictionaryRenderer, LanguageTemplateDictionary, TemplateFunction } from "../templateManager/dictionaryRenderer";
import { TemplateManager } from "../templateManager/templateManager";

const introCard = require("./resources/Intro.json");
const resourcesPath = require.resolve("./resources/MainStrings.resx");

export class MainResponses extends TemplateManager {
    // Constants
    public static readonly Cancelled: string = "cancelled";
    public static readonly Completed: string = "completed";
    public static readonly Confused: string = "confused";
    public static readonly Greeting: string = "greeting";
    public static readonly Help: string = "help";
    public static readonly Intro: string = "intro";

    public static sendIntroCard(turnContext: TurnContext, data: any): Promise<Activity> {
        const response = ActivityEx.createReply(turnContext.activity);

        response.attachments = [{
            content: introCard,
            contentType: "application/vnd.microsoft.card.adaptive",
        }];

        return Promise.resolve(response);
    }

    public static async sendHelpCard(turnContext: TurnContext, data: any): Promise<Activity> {
        const response = ActivityEx.createReply(turnContext.activity);
        const title = await this.resources.get("HELP_TITLE");
        const text = await this.resources.get("HELP_TEXT");

        response.attachments = [CardFactory.heroCard(
            title,
            text,
            undefined,
            [
                {
                    title: "Test LUIS",
                    type: ActionTypes.ImBack,
                    value: "hello",
                },
                {
                    title: "Test QnAMaker",
                    type: ActionTypes.ImBack,
                    value: "What is the sdk v4?",
                },
                {
                    title: "Learn More",
                    type: ActionTypes.OpenUrl,
                    value: "https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0",
                },
            ],
        )];

        return Promise.resolve(response);
    }

    private static readonly resources: ResourceParser = new ResourceParser(resourcesPath);

    private static readonly _responseTemplates: LanguageTemplateDictionary = new Map([
        ["default", new Map([
            [MainResponses.Cancelled, MainResponses.fromResources("CANCELLED")],
            [MainResponses.Completed, MainResponses.fromResources("COMPLETED")],
            [MainResponses.Confused, MainResponses.fromResources("CONFUSED")],
            [MainResponses.Greeting, MainResponses.fromResources("GREETING")],
            [MainResponses.Help, (context: TurnContext, data: any) => MainResponses.sendHelpCard(context, data)],
            [MainResponses.Intro, (context: TurnContext, data: any) => MainResponses.sendIntroCard(context, data)],
        ])],
        ["en", undefined],
        ["fr", undefined],
    ]);

    private static fromResources(name: string): TemplateFunction {
        return (context: TurnContext, data: any) => MainResponses.resources.get(name);
    }

    constructor() {
        super();
        this.register(new DictionaryRenderer(MainResponses._responseTemplates));
    }
}
