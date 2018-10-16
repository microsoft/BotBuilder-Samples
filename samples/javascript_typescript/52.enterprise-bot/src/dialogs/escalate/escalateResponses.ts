// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { TurnContext } from "botbuilder";
import { ResourceParser } from "../shared/resourceParser";
import { DictionaryRenderer, LanguageTemplateDictionary, TemplateFunction } from "../templateManager/dictionaryRenderer";
import { TemplateManager } from "../templateManager/templateManager";
const resourcesPath = require.resolve("./resources/EscalateStrings.resx");

export class EscalateResponses extends TemplateManager {
    public static readonly SendPhone: string = "sendPhone";

    private static readonly resources: ResourceParser = new ResourceParser(resourcesPath);

    private static readonly _responseTemplates: LanguageTemplateDictionary = new Map([
        ["default", new Map([
            [EscalateResponses.SendPhone, EscalateResponses.fromResources("PHONE_INFO")],
        ])],
        ["en", undefined],
        ["fr", undefined],
    ]);

    private static fromResources(name: string): TemplateFunction {
        return (context: TurnContext, data: any) => EscalateResponses.resources.get(name);
    }

    constructor() {
        super();
        this.register(new DictionaryRenderer(EscalateResponses._responseTemplates));
    }
}
