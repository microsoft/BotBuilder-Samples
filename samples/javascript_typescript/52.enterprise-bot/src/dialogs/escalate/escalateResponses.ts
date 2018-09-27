// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { TemplateManager } from "../templateManager/templateManager";
import { LanguageTemplateDictionary, DictionaryRenderer, TemplateFunction } from "../templateManager/dictionaryRenderer";
import { ResourceParser } from '../shared/resourceParser';
import { TurnContext } from "botbuilder";
const resourcesPath = require.resolve('./resources/escalateStrings.resx');

export class EscalateResponses extends TemplateManager {
    public static readonly SendPhone: string = 'sendPhone';

    private static readonly resources: ResourceParser = new ResourceParser(resourcesPath);

    private static fromResources(name: string): TemplateFunction {
        return (context: TurnContext, data: any) => EscalateResponses.resources.get(name);
    }

    private static readonly _responseTemplates: LanguageTemplateDictionary = new Map([
        ['default', new Map([
            [ EscalateResponses.SendPhone, EscalateResponses.fromResources('PHONE_INFO')],
        ])],
        ['en', undefined],
        ['fr', undefined]
    ]);

    constructor() {
        super();
        this.Register(new DictionaryRenderer(EscalateResponses._responseTemplates));
    }
}