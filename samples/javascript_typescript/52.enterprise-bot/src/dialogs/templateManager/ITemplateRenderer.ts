// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { TurnContext } from "botbuilder-core";

/// <summary>
/// Defines interface for data binding to template and rendering a string
/// </summary>
export interface ITemplateRenderer {
    /// <summary>
    /// render a template to an activity or string
    /// </summary>
    /// <param name="turnContext">context</param>
    /// <param name="language">language to render</param>
    /// <param name="templateId">tenmplate to render</param>
    /// <param name="data">data object to use to render</param>
    /// <returns></returns>
    renderTemplate(turnContext: TurnContext, language: string, templateId: string, data: any): Promise<any>;
}
