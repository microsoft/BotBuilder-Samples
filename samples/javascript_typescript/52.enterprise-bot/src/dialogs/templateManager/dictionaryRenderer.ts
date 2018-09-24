// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { ITemplateRenderer } from './ITemplateRenderer';
import { TurnContext } from 'botbuilder-core';

/// <summary>
/// Map of Template Ids-> Template Function()
/// </summary>
export declare type TemplateIdMap = Map<string, (turnContext: TurnContext, data: object) => object>;

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
///   templateManager.Register(new DictionaryRenderer(myTemplates))
/// </summary>
export class DictionaryRenderer implements ITemplateRenderer {
    private _languages: LanguageTemplateDictionary;

    constructor(templates: LanguageTemplateDictionary) {
        this._languages = templates;
    }

    public RenderTemplate(turnContext: TurnContext, language: string, templateId: string, data: object): Promise<object | null> {
        let templates: TemplateIdMap | undefined = this._languages.get(language);
        if (templates) {
            let template: ((turnContext: TurnContext, data: object) => object) | undefined = templates.get(templateId);
            if (template) {
                let result: object = template(turnContext, data);
                if (result != null) {
                    return Promise.resolve(result as object);
                }
            }
        }

        return Promise.resolve(null);
    }
}