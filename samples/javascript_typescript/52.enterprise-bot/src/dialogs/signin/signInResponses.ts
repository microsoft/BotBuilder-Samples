// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

import { TurnContext } from "botbuilder";
import { ResourceParser } from "../shared/resourceParser";
import { DictionaryRenderer, LanguageTemplateDictionary, TemplateFunction } from "../templateManager/dictionaryRenderer";
import { TemplateManager } from "../templateManager/templateManager";
const resourcesPath = require.resolve("./resources/SignInStrings.resx");

export class SignInResponses extends TemplateManager {
    // Constants
    public static readonly SignInPrompt: string = "namePrompt";
    public static readonly Succeeded: string = "haveName";
    public static readonly Failed: string = "emailPrompt";

    private static readonly resources: ResourceParser = new ResourceParser(resourcesPath);

    // Fields
    private static readonly _responseTemplates: LanguageTemplateDictionary = new Map([
        ["default", new Map([
            [SignInResponses.SignInPrompt, SignInResponses.fromResources("PROMPT")],
            [SignInResponses.Failed, SignInResponses.fromResources("FAILED")],
            [SignInResponses.Succeeded, async (context: TurnContext, data: any) => {
                const value = await SignInResponses.resources.get("SUCCEEDED");
                return value.replace("{0}", data.name);
            }],
        ])],
        ["en", undefined],
        ["fr", undefined],
    ]);

    private static fromResources(name: string): TemplateFunction {
        return (context: TurnContext, data: any) => SignInResponses.resources.get(name);
    }

    constructor() {
        super();
        this.register(new DictionaryRenderer(SignInResponses._responseTemplates));
    }
}
