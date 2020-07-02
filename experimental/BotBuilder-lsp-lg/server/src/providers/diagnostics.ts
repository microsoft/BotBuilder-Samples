/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

import {
	Diagnostic,	DiagnosticSeverity,	Files,	Connection
} from 'vscode-languageserver';

import { TemplatesStatus, TemplatesEntity } from '../templatesStatus';
import * as util from '../util';
import * as path from 'path';

import { TextDocument } from 'vscode-languageserver-textdocument';
import { Templates } from 'botbuilder-lg';


export async function updateDiagnostics(document: TextDocument, connection: Connection): Promise<void> {
	
    if(!util.isLgFile(path.basename(document.uri)) ) 
		return;
	
    const confDiagLevel = await connection.workspace.getConfiguration({
		scopeUri: document.uri, 
		section : 'LG.Expression.ignoreUnknownFunction'
	}); //.then(((value: string) => value));
    const confCustomFuncListSetting : string = await connection.workspace.getConfiguration({
		scopeUri: document.uri, 
		section: 'LG.Expression.customFunctionList'
	}).then(((value: string) => value == null ? '' : value));    
	
	const engine: Templates = Templates.parseText(document.getText(), Files.uriToFilePath(document.uri));
    const diagnostics = engine.diagnostics;

    TemplatesStatus.templatesMap.set(Files.uriToFilePath(document.uri)!, new TemplatesEntity(document.uri, engine));

	const lspDiagnostics: Diagnostic[] = [];
    let customFunctionList: string[] = [];
    if (confCustomFuncListSetting.length >= 1) {
        customFunctionList = confCustomFuncListSetting.split(",").map(u => u.trim());
    }
    diagnostics.forEach(u => {
        const isUnkownFuncDiag: boolean = u.message.includes("it's not a built-in function or a custom function");
        
		let severity : DiagnosticSeverity;
		switch(u.severity) {
			case 0: 
				severity = DiagnosticSeverity.Error;					
				break;
			case 1:
				severity = DiagnosticSeverity.Warning;
				break;
			case 2:
				severity = DiagnosticSeverity.Information;
				break;
			case 3:
				severity = DiagnosticSeverity.Hint;
				break;
			default:
				severity = DiagnosticSeverity.Error;
				break;
		}

		if (isUnkownFuncDiag === true){
            let ignored = false;
            const funcName = extractFuncName(u.message);
            if (customFunctionList.includes(funcName)) {
                ignored = true;
            } else {
                switch (confDiagLevel) {
                    case "ignore":
                        if (isUnkownFuncDiag) {
                            ignored = true;
                        }
                        break;
                
                    case "warn":
                            if (isUnkownFuncDiag) {
                                u.severity = DiagnosticSeverity.Warning;
                            }
                        break;
                    default:
                        break;
                }
			}

            if (ignored === false){
                const diagItem = Diagnostic.create(
                    {
                        start: {
							line: u.range.start.line - 1 < 0 ? 0 : u.range.start.line - 1, 
							character: u.range.start.character
						},
                        end: {
							line: u.range.end.line - 1 < 0 ? 0 : u.range.end.line - 1, 
							character: u.range.end.character
						},
					},
					u.message,
                    severity
				);
                lspDiagnostics.push(diagItem);
            }
        } else {
            const diagItem = Diagnostic.create(
                {
					start: {
						line: u.range.start.line - 1 < 0 ? 0 : u.range.start.line - 1, 
						character: u.range.start.character
					},
					end: {
						line: u.range.end.line - 1 < 0 ? 0 : u.range.end.line - 1, 
						character: u.range.end.character
					}
				},
                u.message,
                severity
            );
            lspDiagnostics.push(diagItem);
        }
	});
	
	// Send the computed diagnostics to VSCode.
	connection.sendDiagnostics({ uri: document.uri, diagnostics: lspDiagnostics });
}

function extractFuncName(errorMessage: string): string {
    const message = errorMessage.match(/'\.\s([\w][\w0\-.9_]*)\s+does\snot\shave/)![1];
    return message;
}