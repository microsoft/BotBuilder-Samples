import { TurnContext } from 'botbuilder';
import { ResourceParser } from '../shared/resourceParser';
import { DictionaryRenderer, LanguageTemplateDictionary, TemplateFunction } from '../templateManager/dictionaryRenderer';
import { TemplateManager } from '../templateManager/templateManager';
const resourcesPath = require.resolve('./resources/OnboardingStrings.resx');

export class OnboardingResponses extends TemplateManager {
    // Constants
    public static readonly NamePrompt: string = 'namePrompt';
    public static readonly HaveName: string = 'haveName';
    public static readonly EmailPrompt: string = 'emailPrompt';
    public static readonly HaveEmail: string = 'haveEmail';
    public static readonly LocationPrompt: string = 'locationPrompt';
    public static readonly HaveLocation: string = 'haveLocation';

    private static readonly resources: ResourceParser = new ResourceParser(resourcesPath);

    private static fromResources(name: string): TemplateFunction {
        return (context: TurnContext, data: any) => OnboardingResponses.resources.get(name);
    }

    private static readonly _responseTemplates: LanguageTemplateDictionary = new Map([
        ['default', new Map([
            [OnboardingResponses.NamePrompt, OnboardingResponses.fromResources('NAME_PROMPT')],
            [OnboardingResponses.HaveName, async (context: TurnContext, data: any) => {
                const value = await OnboardingResponses.resources.get('HAVE_NAME');
                return value.replace('{0}', data.name);
            }],
            [OnboardingResponses.EmailPrompt, OnboardingResponses.fromResources('EMAIL_PROMPT')],
            [OnboardingResponses.HaveEmail, async (context: TurnContext, data: any) => {
                const value = await OnboardingResponses.resources.get('HAVE_EMAIL');
                return value.replace('{0}', data.email);
            }],
            [OnboardingResponses.LocationPrompt, OnboardingResponses.fromResources('LOCATION_PROMPT')],
            [OnboardingResponses.HaveLocation, async (context: TurnContext, data: any) => {
                const value = await OnboardingResponses.resources.get('HAVE_LOCATION');
                return value.replace('{0}', data.name).replace('{1}', data.location);
            }],
        ])],
        ['en', undefined],
        ['fr', undefined]
    ]);
    
    constructor() {
        super();
        this.register(new DictionaryRenderer(OnboardingResponses._responseTemplates));
    }
}