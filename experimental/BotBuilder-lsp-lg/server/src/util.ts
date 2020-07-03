
import { TextDocument, Range, Position } from "vscode-languageserver-textdocument";
import { DocumentUri, Files, TextDocuments, WorkspaceFolder } from 'vscode-languageserver';
import { Templates, } from "botbuilder-lg";
import { TemplatesStatus, TemplatesEntity } from "./templatesStatus";
import { ReturnType } from "adaptive-expressions";
import { buildInfunctionsMap, FunctionEntity } from './buildinFunctions';

import * as fs from 'fs';
import * as path from 'path';

export function isLgFile(fileName: string): boolean {
    if(fileName === undefined || !fileName.toLowerCase().endsWith('.lg')) {
        return false;
    }
    return true;
}

export function isInFencedCodeBlock(doc: TextDocument, position: Position): boolean {
    const textBefore = doc.getText({ start: {line: 0, character: 0}, end: position });
    const matches = textBefore.match(/```/gm);
    if (matches == null) {
        return false;
    } else {
        return matches.length % 2 != 0;
    }
}

export function getTemplatesFromCurrentLGFile(lgFileUri: DocumentUri) : Templates {

    let result = new Templates();
    const engineEntity: TemplatesEntity | undefined = TemplatesStatus.templatesMap.get(Files.uriToFilePath(lgFileUri)!);
    if (engineEntity !== undefined && engineEntity.templates.allTemplates.length > 0) {
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
        case ReturnType.Array: result = "array";break;
    }

    return result;
}

export function getAllFunctions(lgFileUri: DocumentUri): Map<string, FunctionEntity> {
    const functions: Map<string, FunctionEntity> = new Map<string, FunctionEntity>();

    for (const func of buildInfunctionsMap) {
        functions.set(func[0],func[1]);
    }

    const templates: Templates = getTemplatesFromCurrentLGFile(lgFileUri);

    for (const template of templates.allTemplates) {
        const functionEntity = new FunctionEntity(template.parameters, ReturnType.Object, `Template reference\r\n ${template.body}`);
        let templateName = template.name;
        if (buildInfunctionsMap.has(template.name)) {
            templateName = 'lg.' + template.name;
        }
        functions.set(templateName, functionEntity);
    }

    return functions;
}


export function getFunctionEntity(lgFileUri: DocumentUri, name: string): FunctionEntity|undefined {
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

export function getWordRangeAtPosition(document: TextDocument, position: Position): Range|null {
	const firstPart = /[a-zA-Z0-9_.]+$/.exec(document.getText({start: document.positionAt(0), end: position}));
	const secondPart = /^[a-zA-Z0-9_.]+/.exec(document.getText({start: position, end: document.positionAt(document.getText().length-1)}));
	
	if (!firstPart && !secondPart) {
		return null;
	}

	const startPosition = firstPart==null?null: document.positionAt(document.offsetAt(position) - firstPart[0].length);
	const endPosition = secondPart==null?null: document.positionAt(document.offsetAt(position) + secondPart[0].length);

	const wordRange : Range = {
		start: startPosition==null?position:startPosition,
		end: endPosition==null?position:endPosition
	};
	return wordRange;
}

export function triggerLGFileFinder(workspaceFolders: WorkspaceFolder[]) {
    TemplatesStatus.lgFilesOfWorkspace = [];
    workspaceFolders.forEach(workspaceFolder => fs.readdir(Files.uriToFilePath(workspaceFolder.uri)!, (err, files) => {
        if(err) {
            console.log(err);
        } else {
            TemplatesStatus.lgFilesOfWorkspace = [];
            files.filter(file => isLgFile(file)).forEach(file => {
                TemplatesStatus.lgFilesOfWorkspace.push(path.join(Files.uriToFilePath(workspaceFolder.uri)!, file));
            });
        }
    }));
}

export const cardPropDict = {
    CardAction: [
        {name:'title', placeHolder:'{your_title}'},
        {name:'type', placeHolder:'imBack'}, 
        {name:'value', placeHolder:'{your_value}'}],
    Cards: [
        {name:'text', placeHolder:'{text}'},
        {name:'buttons', placeHolder:'{button_list}'}],
    Attachment: [
        {name:'contenttype', placeHolder:'herocard'},
        {name:'content', placeHolder:'{attachment_content}'}],
    Others: [
        {name:'type', placeHolder:'{typename}'},
        {name:'value', placeHolder:'{value}'}],
    Activity: [
        {name:'text', placeHolder:'{text_result}'}]
  };


  export const cardPropDictFull = {
    CardAction: [
        {name:'type', placeHolder:'imBack'},
        {name:'title'}, 
        {name:'image'},
        {name:'text'},
        {name:'displayText'},
        {name:'channelData'},
        {name:'image'},
        {name:'image'},
        {name:'image'}],
    HeroCard: [
        {name:'title', placeHolder:'{your_title}'},
        {name:'subtitle', placeHolder:'{your_subtitle}'},
        {name:'text', placeHolder:'{text}'},
        {name:'buttons', placeHolder:'{button_list}'},
        {name:'images', placeHolder:'{image_list}'},
        {name:'tap', placeHolder:'{tap}'}],
    ThumbnailCard: [
        {name:'title', placeHolder:'{your_title}'},
        {name:'subtitle', placeHolder:'{your_subtitle}'},
        {name:'text', placeHolder:'{text}'},
        {name:'buttons', placeHolder:'{button_list}'},
        {name:'images', placeHolder:'{image_list}'},
        {name:'tap', placeHolder:'{tap}'}],
    AudioCard: [
        {name:'title', placeHolder:'{your_title}'},
        {name:'subtitle', placeHolder:'{your_subtitle}'},
        {name:'text', placeHolder:'{text}'},
        {name:'media', placeHolder:'{media_list}'},
        {name:'buttons', placeHolder:'{button_list}'},
        {name:'shareable', placeHolder:'false'},
        {name:'autoloop', placeHolder:'false'},
        {name:'autostart', placeHolder:'false'},
        {name:'aspect'},
        {name:'image'},
        {name:'duration'},
        {name:'value'}],
    VideoCard: [
        {name:'title', placeHolder:'{your_title}'},
        {name:'subtitle', placeHolder:'{your_subtitle}'},
        {name:'text', placeHolder:'{text}'},
        {name:'media', placeHolder:'{media_list}'},
        {name:'buttons', placeHolder:'{button_list}'},
        {name:'shareable', placeHolder:'false'},
        {name:'autoloop', placeHolder:'false'},
        {name:'autostart', placeHolder:'false'},
        {name:'aspect'},
        {name:'image'},
        {name:'duration'},
        {name:'value'}],
    AnimationCard: [
        {name:'title', placeHolder:'{your_title}'},
        {name:'subtitle', placeHolder:'{your_subtitle}'},
        {name:'text', placeHolder:'{text}'},
        {name:'media', placeHolder:'{media_list}'},
        {name:'buttons', placeHolder:'{button_list}'},
        {name:'shareable', placeHolder:'false'},
        {name:'autoloop', placeHolder:'false'},
        {name:'autostart', placeHolder:'false'},
        {name:'aspect'},
        {name:'image'},
        {name:'duration'},
        {name:'value'}],
    SigninCard: [
        {name:'text', placeHolder:'{text}'},
        {name:'buttons', placeHolder:'{button_list}'}],
    OAuthCard: [
        {name:'text', placeHolder:'{text}'},
        {name:'buttons', placeHolder:'{button_list}'},
        {name:'connectionname'}],
    ReceiptCard: [
        {name:'title'},
        {name:'facts'},
        {name:'items'},
        {name:'tap'},
        {name:'total'},
        {name:'tax'},
        {name:'vat'},
        {name:'buttons'}],
    Attachment: [
        {name:'contenttype', placeHolder:'herocard'},
        {name:'content', placeHolder:'{attachment_content}'}],
    Others: [
        {name:'type', placeHolder:'{typename}'},
        {name:'value', placeHolder:'{value}'}],
    Activity: [
        {name:'type'},
        {name:'textFormat'},
        {name:'attachmentLayout'},
        {name:'topicName'},
        {name:'locale'},
        {name:'text', placeHolder:'{text_result}'},
        {name:'speak', placeHolder:'{speak_result}'},
        {name:'inputHint'},
        {name:'summary'},
        {name:'suggestedActions'},
        {name:'attachments'},
        {name:'entities'},
        {name:'channelData'},
        {name:'action'},
        {name:'label'},
        {name:'valueType'},
        {name:'value'},
        {name:'name'},
        {name:'code'},
        {name:'importance'},
        {name:'deliveryMode'},
        {name:'textHighlights'},
        {name:'semanticAction'},
    ]
  };

export const cardTypes = [
    'HeroCard',
    'SigninCard',
    'ThumbnailCard',
    'AudioCard',
    'VideoCard',
    'AnimationCard',
    'MediaCard',
    'OAuthCard',
    'Attachment',
    'CardAction',
    'AdaptiveCard',
    'Activity',
];