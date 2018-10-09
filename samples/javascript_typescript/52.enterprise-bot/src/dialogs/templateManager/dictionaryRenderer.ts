// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { TurnContext } from "botbuilder-core";
import { ITemplateRenderer } from "./ITemplateRenderer";

export declare type TemplateFunction = (turnContext: TurnContext, data: any) => Promise<any>;

/// <summary>
/// Map of Template Ids-> Template Function()
/// </summary>
export declare type TemplateIdMap = Map<string, TemplateFunction>;

/// <summary>
/// Map of language -> template functions
/// </summary>
export declare type LanguageTemplateDictionary = Map<string, TemplateIdMap | undefined>;

/// <summary>
///   This is a simple template engine which has a resource map of template functions
///  let myTemplates  = {
///       "en" : {
///         "templateId": (context, data) => $"your name  is {data.name}",
///         "templateId": (context, data) => { return new Activity(); }
///     }`
///  }
///  }
///   To use, simply register with templateManager
///   templateManager.register(new DictionaryRenderer(myTemplates))
/// </summary>
export class DictionaryRenderer implements ITemplateRenderer {
    private _languages: LanguageTemplateDictionary;

    constructor(templates: LanguageTemplateDictionary) {
        this._languages = templates;
    }

    public renderTemplate(turnContext: TurnContext, language: string, templateId: string, data: any): Promise<any> {
        const templates: TemplateIdMap | undefined = this._languages.get(language);
        if (templates) {
            const template: TemplateFunction | undefined = templates.get(templateId);
            if (template) {
                const result = template(turnContext, data);
                if (result) {
                    return result;
                }
            }
        }

        return Promise.resolve(undefined);
    }
}
