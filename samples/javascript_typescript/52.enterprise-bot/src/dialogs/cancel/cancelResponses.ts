// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.import { CancelResponses } from './cancelResponeses';

import { TurnContext } from "botbuilder";
import { ResourceParser } from "../shared/resourceParser";
import { DictionaryRenderer, LanguageTemplateDictionary, TemplateFunction } from "../templateManager/dictionaryRenderer";
import { TemplateManager } from "../templateManager/templateManager";
const resourcesPath = require.resolve("./resources/CancelStrings.resx");

export class CancelResponses extends TemplateManager {
    // Constants
    public static readonly _confirmPrompt: string = "Cancel.ConfirmCancelPrompt";
    public static readonly _cancelConfirmed: string = "Cancel.CancelConfirmed";
    public static readonly _cancelDenied: string = "Cancel.CancelDenied";

    private static readonly resources: ResourceParser = new ResourceParser(resourcesPath);

    // Fields
    private static readonly _responseTemplates: LanguageTemplateDictionary = new Map([
        ["default", new Map([
            [CancelResponses._confirmPrompt, CancelResponses.fromResources("CANCEL_PROMPT")],
            [CancelResponses._cancelConfirmed, CancelResponses.fromResources("CANCEL_CONFIRMED")],
            [CancelResponses._cancelDenied, CancelResponses.fromResources("CANCEL_DENIED")],
        ])],
        ["en", undefined],
        ["fr", undefined],
    ]);

    private static fromResources(name: string): TemplateFunction {
        return (context: TurnContext, data: any) => CancelResponses.resources.get(name);
    }

    constructor() {
        super();
        this.register(new DictionaryRenderer(CancelResponses._responseTemplates));
    }
}
