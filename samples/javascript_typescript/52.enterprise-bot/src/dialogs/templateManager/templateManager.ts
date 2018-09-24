// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { ITemplateRenderer } from './ITemplateRenderer';
import { Activity, ActivityTypes } from 'botframework-schema';
import { TurnContext } from 'botbuilder-core';
export class TemplateManager {
    private _templateRenderers: ITemplateRenderer[] = [];
    private _languageFallback: string[] = [];

    constructor() {
    }

    /// <summary>
    /// Add a template engine for binding templates
    /// </summary>
    /// <param name="renderer"></param>
    public Register(renderer: ITemplateRenderer): TemplateManager {
        if (!this._templateRenderers.some(x => x == renderer))
            this._templateRenderers.push(renderer);

        return this;
    }

    /// <summary>
    /// List registered template engines
    /// </summary>
    /// <returns></returns>
    public List(): ITemplateRenderer[] {
        return this._templateRenderers;
    }

    public SetLanguagePolicy(languageFallback: string[]): void {
        this._languageFallback = languageFallback;
    }

    public GetLanguagePolicy(): string[] {
        return this._languageFallback;
    }

    /// <summary>
    /// Send a reply with the template
    /// </summary>
    /// <param name="turnContext"></param>
    /// <param name="templateId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async ReplyWith(turnContext: TurnContext, templateId: string, data: object | null = null): Promise<void> {
        if (turnContext == null) throw new Error('turnContext is null');

        // apply template
        let boundActivity: Activity | null = await this.RenderTemplate(turnContext, turnContext.activity.locale, templateId, data);
        if (boundActivity != null) {
            await turnContext.sendActivity(boundActivity);
            return;
        }
        
        return;
    }


    /// <summary>
    /// Render the template
    /// </summary>
    /// <param name="turnContext"></param>
    /// <param name="language"></param>
    /// <param name="templateId"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public async RenderTemplate(turnContext: TurnContext, language: string | undefined, templateId: string, data: object | null = null): Promise<Activity | null> {
        let fallbackLocales = this._languageFallback;

        if (!language) {
            fallbackLocales.push(<string>language);
        }

        fallbackLocales.push("default");

        // try each locale until successful
        for (let locale of fallbackLocales) {
            for (let renderer of this._templateRenderers) {
                let templateOutput: object | null = await renderer.RenderTemplate(turnContext, locale, templateId, data);
                if (templateOutput != null) {
                    if (templateOutput instanceof String) {
                        return <Activity>{ type: ActivityTypes.Message, text: <string>templateOutput };
                    }
                    else {
                        return templateOutput as Activity;
                    }
                }
            }
        }
        return null;
    }
}
