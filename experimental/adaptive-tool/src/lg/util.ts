/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import { TextDocument, Range, Position } from "vscode";
import { Templates, } from "botbuilder-lg";
import { TemplatesStatus, TemplatesEntity } from "./templatesStatus";
import * as vscode from 'vscode';
import { ReturnType } from "adaptive-expressions";
import { buildInfunctionsMap, FunctionEntity } from './buildinFunctions';

export function isLgFile(fileName: string): boolean {
    if(fileName === undefined || !fileName.toLowerCase().endsWith('.lg')) {
        return false;
    }
    return true;
}

export function isLuFile(fileName: string): boolean {
    if(fileName === undefined || !fileName.toLowerCase().endsWith('.lu')) {
        return false;
    }
    return true;
}

export function isInFencedCodeBlock(doc: TextDocument, position: Position): boolean {
    let textBefore = doc.getText(new Range(new Position(0, 0), position));
    let matches = textBefore.match(/```/gm);
    if (matches == null) {
        return false;
    } else {
        return matches.length % 2 != 0;
    }
}

export function getTemplatesFromCurrentLGFile(lgFileUri: vscode.Uri) : Templates {

    let result = new Templates();
    let engineEntity: TemplatesEntity = TemplatesStatus.templatesMap.get(lgFileUri.fsPath);
    if (engineEntity !== undefined && engineEntity.templates.toArray().length > 0) {
        result = engineEntity.templates;
    }

    return result;
}

export function getreturnTypeStrFromReturnType(returnType: ReturnType): string {
    let result = '';
    switch(returnType) {
        case ReturnType.Boolean: result = "boolean";break;
        case ReturnType.Number: result = "number";break;
        case ReturnType.Object: result = "any";break;
        case ReturnType.String: result = "string";break;
    }

    return result;
}

export function getAllFunctions(lgFileUri: vscode.Uri): Map<string, FunctionEntity> {
    const functions: Map<string, FunctionEntity> = new Map<string, FunctionEntity>();

    for (const func of buildInfunctionsMap) {
        functions.set(func[0],func[1]);
    }

    const templates: Templates = getTemplatesFromCurrentLGFile(lgFileUri);

    for (const template of templates) {
        var functionEntity = new FunctionEntity(template.parameters, ReturnType.Object, 'Template reference');
        let templateName = template.name;
        if (buildInfunctionsMap.has(template.name)) {
            templateName = 'lg.' + template.name;
        }
        functions.set(templateName, functionEntity);
    }

    return functions;
}


export function getFunctionEntity(lgFileUri: vscode.Uri, name: string): FunctionEntity|undefined {
    const allFunctions = getAllFunctions(lgFileUri);

    if (allFunctions.has(name)) {
        return allFunctions.get(name);
    } else if (name.startsWith('lg.')){
        const pureName = name.substr('lg.'.length);
        if (allFunctions.has(pureName)) {
            return allFunctions.get(pureName);
        }
    } else {
        const lgWordName = 'lg.' + name;
        if (allFunctions.has(lgWordName)) {
            return allFunctions.get(lgWordName);
        }
    }
    return undefined;
}

export const cardPropDict = {
    CardAction: ['title', 'type', 'value'],
    Suggestions: ['SuggestionActions'],
    Cards: ['title', 'subtitle', 'text', 'image', 'buttons'],
    Attachment: ['contenttype', 'content'],
    Others: ['type', 'name', 'value'],
  };

export const cardTypes = [
    'Typing',
    'Suggestions',
    'HeroCard',
    'SigninCard',
    'ThumbnailCard',
    'AudioCard',
    'VideoCard',
    'AnimationCard',
    'MediaCard',
    'OAuthCard',
    'Attachment',
    'AttachmentLayout',
    'CardAction',
    'AdaptiveCard',
    'Activity',
];