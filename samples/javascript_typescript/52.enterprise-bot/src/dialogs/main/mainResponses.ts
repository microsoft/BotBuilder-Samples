import { TemplateManager } from "../templateManager/templateManager";
import { LanguageTemplateDictionary, DictionaryRenderer, TemplateFunction } from "../templateManager/dictionaryRenderer";
import { TurnContext, Activity } from "botbuilder";
import { ResourceParser } from '../shared/resourceParser';
import { ActivityEx } from "../../utils/activityEx";
const resourcesPath = require.resolve('./resources/mainResponses.resx');

export class MainResponses extends TemplateManager {
    // Constants
    public static readonly Cancelled: string = 'cancelled';
    public static readonly Completed: string = 'completed';
    public static readonly Confused: string = 'confused';
    public static readonly Greeting: string = 'greeting';
    public static readonly Help: string = 'help';
    public static readonly Intro: string = 'intro';

    private static readonly resources: ResourceParser = new ResourceParser(resourcesPath);

    private static fromResources(name: string): TemplateFunction {
        return (context: TurnContext, data: any) => MainResponses.resources.get(name);
    }

    private static readonly _responseTemplates: LanguageTemplateDictionary = new Map([
        ['default', new Map([
            [MainResponses.Cancelled, MainResponses.fromResources('CANCELLED')],
            [MainResponses.Completed, MainResponses.fromResources('COMPLETED')],
            [MainResponses.Confused, MainResponses.fromResources('CONFUSED')],
            [MainResponses.Greeting, MainResponses.fromResources('GREETING')],
            [MainResponses.Help, (context: TurnContext, data: any) => MainResponses.sendHelpCard(context, data)]
        ])],
        ['en', undefined],
        ['fr', undefined]
    ]);

    constructor() {
        super();
        this.Register(new DictionaryRenderer(MainResponses._responseTemplates));
    }

    public static sendIntroCard(turnContext: TurnContext, data: any): Promise<Activity> {
        const response = ActivityEx.createReply(turnContext.activity);
        //TODO create card
        return Promise.resolve(response);
    }

    public static sendHelpCard(turnContext: TurnContext, data: any): Promise<Activity> {
        const response = turnContext.activity;
        //TODO create card
        return Promise.resolve(response);
    }
}